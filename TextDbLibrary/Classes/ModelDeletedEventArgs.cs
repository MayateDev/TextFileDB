using System;

namespace TextDbLibrary.Classes
{
    internal class EntityDeletedEventArgs : EventArgs
    {
        internal EntityDeletedEventArgs()
        {

        }

        internal EntityDeletedEventArgs(string deletedId, Type deletedType)
        {
            DeletedId = deletedId;
            DeletedType = deletedType;
        }

        internal string DeletedId { get; private set; }
        internal Type DeletedType { get; private set; }
        internal bool DeleteRelationsSucceded { get; set; } = false;
    }
}
