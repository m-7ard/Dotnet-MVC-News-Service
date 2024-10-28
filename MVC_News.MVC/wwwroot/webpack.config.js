const path = require("path");

module.exports = {
    entry: "./js/src/index.ts", // Entry point of your TypeScript files
    output: {
        filename: "bundle.js", // Output bundle file name
        path: path.resolve(__dirname, "js/dist"), // Output directory
        clean: false, // Clean the output directory before each build
    },
    resolve: {
        extensions: [".ts", ".js"], // Resolve these file extensions
    },
    module: {
        rules: [
            {
                test: /\.ts$/, // Transform TypeScript files
                use: [
                    {
                        loader: "babel-loader",
                        options: {
                            presets: [
                                "@babel/preset-env",
                                "@babel/preset-typescript",
                            ],
                        },
                    },
                ],
                exclude: /node_modules/,
            },
        ],
    },
    mode: "development", // Set the mode to development
    devtool: "inline-source-map", // Enable source maps for easier debugging
};
