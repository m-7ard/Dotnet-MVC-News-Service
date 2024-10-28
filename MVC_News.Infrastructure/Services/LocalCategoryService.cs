using MVC_Classifieds.Application.Interfaces.Services;
using MVC_Classifieds.Domain.Entities;

namespace MVC_Classifieds.Infrastructure.Services;

public class LocalCategoryService : ILocalCategoryService
{
    private Category? BaseCategory { get; set; }
    private Dictionary<int, Tuple<Category, List<Category>>> PlainCategories { get; set; }

    public LocalCategoryService()
    {
        BaseCategory = null;
        PlainCategories = new Dictionary<int, Tuple<Category, List<Category>>>();
    }

    public void Initialise(List<Category> categories)
    {

        foreach (var category in categories)
        {
            PlainCategories[category.Id] = new Tuple<Category, List<Category>>(
                category,
                new List<Category>()
            );

            if (category.ParentCategoryId == null)
            {
                if (BaseCategory is not null)
                {
                    throw new InvalidOperationException("Only one category can have 'null' as ParentCategoryId.");
                }
                BaseCategory = category;
            }
        }

        foreach (var category in categories)
        {
            if (category.ParentCategoryId.HasValue)
            {
                var parentTuple = PlainCategories[category.ParentCategoryId.Value];
                parentTuple.Item2.Add(category);
            }
        }
    }

    public void AddCategory(Category category)
    {
        if (category.ParentCategoryId is null)
        {
            if (BaseCategory is not null)
            {
                throw new InvalidOperationException("Cannot add a category with a null parentCategoryId since a BaseCategory already exists.");
            }

            BaseCategory = category;
        }
        else
        {
            var parentTuple = PlainCategories[category.ParentCategoryId.Value];
            parentTuple.Item2.Add(category);
        }

        PlainCategories[category.Id] = new Tuple<Category, List<Category>>(category, new List<Category>());
    }

    public Category? GetBaseCategory()
    {
        return BaseCategory;
    }

    public List<Category> GetSubcategoriesById(int id)
    {
        if (PlainCategories.TryGetValue(id, out Tuple<Category, List<Category>>? value))
        {
            return value.Item2;
        }
        
        return new List<Category>();
    }

    public List<Category> GetLeafCategories()
    {
        return PlainCategories
            .Where(entry => entry.Value.Item2.Count == 0)
            .Select(entry => entry.Value.Item1)
            .ToList();
    }
}