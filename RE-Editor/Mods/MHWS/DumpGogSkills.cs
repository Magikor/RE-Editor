using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models;
using RE_Editor.Windows;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class DumpGogSkills : IMod {
    [UsedImplicitly]
    public static void Make(MainWindow mainWindow) {
        var entryObj = ReDataFile.Read($@"{PathHelper.CHUNK_PATH}{PathHelper.GOG_SKILL_GROUP_DATA_PATH}").rsz.GetEntryObject();

        // Avoid compile-time dependency on specific generated structs. The downloaded RSZ json dump
        // can miss some MHWS types, which would otherwise break the build.
        var valuesProp = entryObj.GetType().GetProperty("Values");
        var valuesObj  = valuesProp?.GetValue(entryObj);
        var values     = valuesObj as IEnumerable;

        if (values == null) {
            MessageBox.Show("Unable to dump Gog skills: entry object does not expose a Values list.", "RE-Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var writer = new StreamWriter(File.Open($@"{PathHelper.MODS_PATH}\..\Dumped Data\Gog Skill Group Data.csv", FileMode.Create, FileAccess.Write, FileShare.Read));
        writer.WriteLine("ArtianSkillType,GroupSkillId,SeriesSkillId,Probability");

        foreach (var row in values.Cast<object>()) {
            var rowType = row.GetType();
            var artianSkillType = rowType.GetProperty("ArtianSkillType")?.GetValue(row);
            var groupSkillIdObj = rowType.GetProperty("GroupSkillId")?.GetValue(row);
            var seriesSkillIdObj = rowType.GetProperty("SeriesSkillId")?.GetValue(row);
            var probability = rowType.GetProperty("Probability")?.GetValue(row);

            if (groupSkillIdObj == null || seriesSkillIdObj == null) continue;

            var groupSkillId = Convert.ToInt32(groupSkillIdObj);
            var seriesSkillId = Convert.ToInt32(seriesSkillIdObj);
            var skillNames = DataHelper.SKILL_NAME_BY_ENUM_VALUE[Global.locale];
            if (!skillNames.TryGetValue(groupSkillId, out var groupName)) groupName = groupSkillId.ToString();
            if (!skillNames.TryGetValue(seriesSkillId, out var seriesName)) seriesName = seriesSkillId.ToString();

            writer.WriteLine($"{artianSkillType},{groupName},{seriesName},{probability}");
        }
        writer.Close();
    }
}