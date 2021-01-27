using System;
using System.Collections.Generic;
using Consts.Enums;
using Serializable;

namespace Model.Structs
{
    public class ItemStruct
    {
        public ItemSerializable Data;
        public RealityEnum.Reality Reality;
        public int Amount;
    }
}