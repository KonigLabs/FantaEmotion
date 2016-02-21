using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.DragDrop
{
    public interface IDragable
    {
        Type DataType { get; }

        void Update(double x, double y);
    }
}
