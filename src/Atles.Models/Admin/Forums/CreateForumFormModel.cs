﻿namespace Atles.Models.Admin.Forums;

public abstract class ForumFormModelBase
{
    public ForumModel Forum { get; set; } = new ForumModel();
    public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    public IList<PermissionSetModel> PermissionSets { get; set; } = new List<PermissionSetModel>();

    public class ForumModel : SiteFormModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }

        public Guid PermissionSetId { get; set; } = Guid.Empty;
    }

    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class PermissionSetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

public class CreateForumFormModel : ForumFormModelBase
{
}

public class UpdateForumFormModel : ForumFormModelBase
{
}
