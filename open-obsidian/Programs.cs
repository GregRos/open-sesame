using System.Diagnostics;

static class Programs {
    public static void Open(string fileName, string explicitArgs = "") {
        try {
            Process process = new();
            process.StartInfo.FileName = fileName; // Set the file name to open
            process.StartInfo.Arguments = explicitArgs; // Set the arguments to pass to the file
            process.StartInfo.UseShellExecute = true; // Detach the process

            process.Start(); // Start the process

            Logger.Info("Notepad launched independently!");
        }
        catch (Exception ex) {
            Logger.Error("An error occurred: " + ex.Message);
        }
    }
}