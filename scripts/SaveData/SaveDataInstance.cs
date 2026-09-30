using System;
using System.Linq;

namespace GalaxyGauntlet.scripts.SaveData
{
    public class SaveDataInstance
    {
        public SaveSlot[] SaveSlots { get; set; } = [];


        public void CreateSaveSlot(string slotName)
        {
            int slotIndex = FindSlotIndex(slotName);
            if(slotIndex == -1)
            {
                SaveSlots = SaveSlots.Append(new SaveSlot(slotName)).ToArray();
            }
            else
            {
                SaveSlots[slotIndex] = new(slotName);
            }
            SaveData.Save();
        }


        public bool SaveSlotExists(string slotName)
        {
            return SaveSlots.Any(e => e.SlotName == slotName);
        }


        public SaveSlot GetSaveSlot(string slotName)
        {
            return SaveSlots.FirstOrDefault(e => e.SlotName == slotName);
        }


        public void WriteSlotData(SaveSlot saveSlot)
        {
            FileLogger.QuietLogMessage("Writing slot data...");
            int slotIndex = FindSlotIndex(saveSlot.SlotName);
            if (slotIndex == -1)
            {
                SaveSlots = SaveSlots.Append(saveSlot).ToArray();
            }
            else
            {
                SaveSlots[slotIndex] = saveSlot;
            }
            SaveData.Save();
        }


        public int FindSlotIndex(string slotName)
        {
            for(int i = 0; i < SaveSlots.Length; i++)
            {
                if (string.Equals(SaveSlots[i].SlotName, slotName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
