/*
    FmodGuids.cs - FMOD Studio API

    Generated GUIDs for project 'VampireSurvivorsRhythm.fspro'
*/

using System;
using System.Collections.Generic;

namespace Audio
{
    public class AudioEvent
    {
        public static readonly FMOD.GUID GameplayMusic = new FMOD.GUID { Data1 = 1369531543, Data2 = 1224165700, Data3 = -1045484356, Data4 = 377330144 };


        public static readonly Dictionary<string, FMOD.GUID> AudioEventNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"GameplayMusic", GameplayMusic}, 
        };
    }

    public class AudioBus
    {
        public static readonly FMOD.GUID MasterBus = new FMOD.GUID { Data1 = -323976756, Data2 = 1138677162, Data3 = -583288656, Data4 = -142521793 };
        public static readonly FMOD.GUID Music = new FMOD.GUID { Data1 = 805202742, Data2 = 1186561971, Data3 = -181328497, Data4 = 1200155848 };
        public static readonly FMOD.GUID Reverb = new FMOD.GUID { Data1 = -38252101, Data2 = 1102538477, Data3 = -819948353, Data4 = -1790549757 };


        public static readonly Dictionary<string, FMOD.GUID> AudioBusNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"MasterBus", MasterBus}, {"Music", Music}, {"Reverb", Reverb}, 
        };
    }

    public class AudioSnapshot
    {
        public static readonly FMOD.GUID FilteredMusic = new FMOD.GUID { Data1 = -1844649376, Data2 = 1114397820, Data3 = -1687499366, Data4 = 1316509500 };


        public static readonly Dictionary<string, FMOD.GUID> AudioSnapshotNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"FilteredMusic", FilteredMusic}, 
        };
    }

    public class AudioBank
    {
        public static readonly FMOD.GUID Master = new FMOD.GUID { Data1 = 598590988, Data2 = 1190612160, Data3 = 553388216, Data4 = 653845446 };


        public static readonly Dictionary<string, FMOD.GUID> AudioBankNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"Master", Master}, 
        };
    }

}

