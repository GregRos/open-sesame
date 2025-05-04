using System.Diagnostics;

public static class Programs {
    public static bool Open(string fileName, string explicitArgs = "") {
        Logger.Info($"Opening {fileName} with arguments: {explicitArgs}");
        try {
            Process process = new();
            process.StartInfo.FileName = fileName; // Set the file name to open
            process.StartInfo.Arguments = explicitArgs; // Set the arguments to pass to the file
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // Hide the window
            process.Start(); // Start the process
            Logger.Info($"Process started with ID: {process.Id}"); // Log the process ID
            return true;
        }
        catch (Exception ex) {
            Logger.Error("An error occurred: " + ex.Message);
            return false;
        }
    }
}