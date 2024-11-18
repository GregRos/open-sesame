static class OpenFirefox {
    static void Main(string[] args) {
        var firefoxPath = Environment.GetEnvironmentVariable("FIREFOX_PATH") ?? @"C:\Program Files\Firefox Developer Edition\firefox.exe";
        Programs.Open(firefoxPath);
    }
}