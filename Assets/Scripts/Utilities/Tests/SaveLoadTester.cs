using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Tests
{
    public class SaveLoadTester : MonoBehaviour
    {
        private static string FileName => $"test_save_{DateTime.Now}";

        [Serializable]
        public class PlayerData
        {
            public string PlayerName = "Hero";
            public int Level = 1;
            public float Health = 100f;
            public bool SoundOn = true;
            public List<string> Inventory = new() { "Sword", "Shield" };
        }

        private float _btnW = 280f, _btnH = 45f, _x = 20f;
        private string _lastResult = "press a button to test";

        private void OnGUI()
        {
            var y = 20f;
            var gap = _btnH + 6f;

            GUI.Label(new Rect(_x, y, 500, 30), "<b>SaveLoadUtility Tester</b>", LabelStyle());
            y += 40f;

            if (Button(y, "1 — Save (default data)"))
            {
                TestSave();
            }
            y += gap;
            if (Button(y, "2 — Load"))
            {
                TestLoad();
            }
            y += gap;
            if (Button(y, "3 — LoadOrDefault (missing file)"))
            {
                TestLoadOrDefault();
            }
            y += gap;
            if (Button(y, "4 — SaveAsync / LoadAsync"))
            {
                _ = TestAsync();
            }
            y += gap;
            if (Button(y, "5 — Save exists?"))
            {
                TestExists();
            }
            y += gap;
            if (Button(y, "6 — Corrupt the save file"))
            {
                TestCorrupt();
            }
            y += gap;
            if (Button(y, "7 — Load (triggers backup restore)"))
            {
                TestLoadAfterCorrupt();
            }
            y += gap;
            if (Button(y, "8 — List all saves"))
            {
                TestListAll();
            }
            y += gap;
            if (Button(y, "9 — Delete save"))
            {
                TestDelete();
            }
            y += gap;
            if (Button(y, "10 — Two independent instances"))
            {
                TestTwoInstances();
            }
            y += gap;

            y += 10f;
            GUI.Box(new Rect(_x, y, 500, 50), "");
            GUI.Label(new Rect(_x + 8f, y + 6f, 600, 100), $"Last: {_lastResult}");
        }


        #region Tests

        private void TestSave()
        {
            var data = new PlayerData
            {
                PlayerName = "PlayerName123",
                Level = 5, 
                Inventory = new List<string> { "Item1", "Item2" }
            };
            var ok = SaveLoadUtility.Default.Save(data, FileName);
            AddLog(ok ? "Save OK" : "Save FAILED", ok);
        }

        private void TestLoad()
        {
            var data = SaveLoadUtility.Default.Load<PlayerData>(FileName);
            if (data != null)
                AddLog(
                    $"Load OK — {data.PlayerName} Lv{data.Level}  HP:{data.Health}  Items:[{string.Join(", ", data.Inventory)}]",
                    true);
            else
                AddLog("Load returned null (no file or corrupted)", false);
        }

        private void TestLoadOrDefault()
        {
            var data = SaveLoadUtility.Default.LoadOrDefault<PlayerData>("nonexistent_file");
            AddLog($"LoadOrDefault — got default instance: {data.PlayerName} Lv{data.Level}", true);
        }

        private async Task TestAsync()
        {
            AddLog("Async save starting…", true);
            var data = new PlayerData { PlayerName = "AsyncHero", Level = 99 };
            var saved = await SaveLoadUtility.Default.SaveAsync(data, FileName + "_async");
            if (!saved)
            {
                AddLog("Async save FAILED", false);
                return;
            }

            var loaded = await SaveLoadUtility.Default.LoadOrDefaultAsync<PlayerData>(FileName + "_async");
            AddLog($"Async round-trip OK — {loaded.PlayerName} Lv{loaded.Level}", true);
        }

        private void TestExists()
        {
            var exists = SaveLoadUtility.Default.SaveExists(FileName);
            AddLog($"SaveExists('{FileName}') = {exists}", exists);
        }

        private void TestCorrupt()
        {
            var path = System.IO.Path.Combine(
                Application.persistentDataPath,
                SaveLoadUtility.Default.SaveDirectoryName,
                FileName + ".json");

            if (!System.IO.File.Exists(path))
            {
                AddLog($"No save file found at: {path}\nRun Test 1 first", false);
                return;
            }

            System.IO.File.WriteAllText(path, "THIS IS NOT VALID JSON !!@#$");
            AddLog("Save file corrupted — now run Test 7", true);
        }

        private void TestLoadAfterCorrupt()
        {
            var data = SaveLoadUtility.Default.Load<PlayerData>(FileName);
            if (data != null)
            {
                AddLog($"Recovered from backup — {data.PlayerName} Lv{data.Level}", true);
            }
            else
            {
                AddLog("Recovery FAILED — run Test 1, then Test 6, then this", false);
            }
        }

        private void TestListAll()
        {
            var saves = SaveLoadUtility.Default.GetAllSaves();
            AddLog(saves.Length == 0
                ? "No saves found"
                : $"Found {saves.Length} save(s): {string.Join(", ", saves)}", true);
        }

        private void TestDelete()
        {
            var ok = SaveLoadUtility.Default.DeleteSave(FileName);
            AddLog(ok ? $"Deleted '{FileName}'" : "Delete failed (file may not exist)", ok);
        }

        private void TestTwoInstances()
        {
            var settingsStore = new SaveLoadUtility
            {
                SaveDirectoryName = "settings"
            };
            settingsStore.Configure("", useEncryption: false);

            var gameStore = new SaveLoadUtility();
            gameStore.Configure("gameplay-secret-key", useEncryption: true);

            var settings = new PlayerData { PlayerName = "SettingsProfile", SoundOn = true };
            var game = new PlayerData { PlayerName = "GameProfile", Level = 42 };

            var a = settingsStore.Save(settings, "user_settings");
            var b = gameStore.Save(game, "slot1");

            AddLog($"Two instances — settings saved:{a}  gameplay saved:{b}  " +
                $"(dirs: 'settings/' and 'saves/')", a && b);
        }

        private void AddLog(string msg, bool success)
        {
            _lastResult = msg;
            if (success)
            {
                Debug.Log($"[SaveLoadTester] success: {msg}");
            }
            else
            {
                Debug.LogWarning($"[SaveLoadTester] fail: {msg}");
            }
        }

        private bool Button(float y, string label)
        {
            return GUI.Button(new Rect(_x, y, _btnW, _btnH), label);
        }

        private static GUIStyle LabelStyle()
        {
            var s = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 14
            };
            return s;
        }

        #endregion
    }
}