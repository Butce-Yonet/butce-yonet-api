using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class NotebookLabelToUserLabel : BaseEntity
{
    public int NotebookLabelId { get; set; }
    public int UserLabelId { get; set; }

    public virtual NotebookLabel NotebookLabel { get; set; }
    public virtual UserLabel UserLabel { get; set; }
}