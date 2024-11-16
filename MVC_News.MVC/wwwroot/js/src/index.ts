declare global {
    var TagField: any;
}

const getElementOrThrow = <T extends Element>(el: HTMLElement, selector: string, errorMessage: string) => {
    const result = el.querySelector<T>(selector);
    if (result == null) {
        throw new Error(errorMessage);
    }

    return result;
};

global.TagField = class TagField {
    private _tags: Set<string>;
    private root: HTMLElement;
    private input: HTMLInputElement;
    private submitButton: HTMLButtonElement;
    private list: HTMLElement;
    private _tagFieldItemTemplate: HTMLElement;

    constructor(root: HTMLElement, initialTags: string[] = []) {
        this.root = root;
        this._tags = new Set(initialTags);  // Initialize with provided tags
        this.input = getElementOrThrow(root, '[role="TAG_FIELD_INPUT"]', "TagField input element does not exist.");
        this.submitButton = getElementOrThrow(root, '[role="TAG_FIELD_SUBMIT"]', "TagField submitButton element does not exist.");
        this._tagFieldItemTemplate = getElementOrThrow(root, '[data-template="TAG_FIELD_ITEM"]', "TagField item template does not exist.");
        this.list = getElementOrThrow(root, '[role="TAG_FIELD_LIST"]', "TagField list element does not exist.");

        this.setUp();

        // Initially render tags
        this.renderInitialTags();
    }

    private setUp() {
        // Add tag on button click
        this.submitButton.addEventListener("click", () => this.addItem());

        // Add tag on enter key press
        this.input.addEventListener("keydown", (event) => {
            event.stopPropagation();
            if (event.key === "Enter") {
                event.preventDefault();
                this.addItem();
            }
        });
    }

    private addItem() {
        const value = this.input.value.trim();
        if (value === "" || this._tags.has(value)) {
            return; // Avoid adding duplicate or empty tags
        }

        this._tags.add(value);
        this.input.value = "";

        // Add only the new item to the DOM
        this.addTagToDOM(value);
    }

    private removeItem(tag: string) {
        if (!this._tags.has(tag)) return;

        this._tags.delete(tag);

        // Remove the item from the DOM
        const node = this.list.querySelector(`[data-tag-value="${tag}"]`);
        if (node) {
            node.remove();
        }
    }

    private renderInitialTags() {
        this._tags.forEach((tag) => {
            this.addTagToDOM(tag);  // Render each initial tag
        });
    }

    private addTagToDOM(tag: string) {
        const node = this._tagFieldItemTemplate.cloneNode(true) as HTMLElement;
        node.innerHTML = node.innerHTML!.replace(/#value/g, tag);
        const input = getElementOrThrow(node, "input", "Input does not exist in tag item template");
        input.removeAttribute("disabled");
        node.classList.remove("hidden");

        // Set a unique identifier for easy DOM querying
        node.setAttribute("data-tag-value", tag);

        // Setup deletion handler
        node.addEventListener("click", () => this.removeItem(tag));

        this.list.appendChild(node);
    }
}

export {};
