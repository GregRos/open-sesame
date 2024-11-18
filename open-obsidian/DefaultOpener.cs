static class Typora {
    public static void Open(string path) {
        Programs.Open($"typora", $"\"{path}\"");
    }
}