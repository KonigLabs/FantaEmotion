using System;

namespace KonigLabs.FantaEmotion.CommonViewModels.DragDrop
{
    public interface IDropable
    {
        Type DataType { get; }

        void Drop(object data);
    }
}
