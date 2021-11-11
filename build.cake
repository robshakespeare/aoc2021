#addin nuget:?package=Cake.FileHelpers&version=4.0.1

var target = Argument<string>("target", "CreateAllDays");
var day = Argument<string>("day", "");
var workingDir = Argument<string>("workingDir", "./.cake-working/");

Task("CreateDay")
    .Does(() =>
    {
        if (string.IsNullOrWhiteSpace(day))
        {
            Error($"Usage: dotnet cake --day=X");
            return;
        }

        var dayPadded = day.PadLeft(2, '0');

        if (DirectoryExists($"./AoC/Day{dayPadded}"))
        {
            Information($"Skipping day {day}, it already exists.");
            return;
        }

        Information($"Creating files for day {day}...");        

        CreateDirectory(workingDir);
        CleanDirectory(workingDir);

        CopyFiles(GetFiles("./Template/**/*.*"), workingDir, true);
        ReplaceTextInFiles("./.cake-working/**/*.*", "NNN", day);
        ReplaceTextInFiles("./.cake-working/**/*.*", "XXX", dayPadded);

        foreach(var file in GetFiles("./.cake-working/**/*.*"))
        {
            var newFilePath = file.GetDirectory().CombineWithFilePath(file.GetFilename().FullPath.Replace("NNN", day));
            MoveFile(file, newFilePath);
        }

        foreach(var dir in GetDirectories("./.cake-working/*/*"))
        {
            var newDirPath = $"./{dir.Segments[dir.Segments.Length-2]}/{dir.GetDirectoryName()}".Replace("XXX", dayPadded);
            MoveDirectory(dir, newDirPath);
        }

        DeleteDirectory(workingDir, new DeleteDirectorySettings { Recursive = true, Force = true });

        Information("Created files for day " + day);
    });

Task("CreateAllDays")
    .Does(() =>
    {
        for(var dayCounter = 0; dayCounter <= 25; dayCounter++)
        {
            day = dayCounter.ToString();
            RunTarget("CreateDay");
        }
    });

RunTarget(target);
