using System;

namespace TextDbLibrary.Classes
{
    internal class EntityDeletedEventArgs : EventArgs
    {
        internal EntityDeletedEventArgs(int deletedId, Type deletedType)
        {
            DeletedId = deletedId;
            DeletedType = deletedType;
        }

        internal int DeletedId { get; private set; }
        internal Type DeletedType { get; private set; }
        internal bool DeleteRelationsSucceded { get; set; } = false;
    }
}
