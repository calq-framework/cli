namespace Calq.Options {
    public class Opts {        
        public static void Load(string[] args, object instance) {
            new Reader(args, instance).Read();
        }
    }
}
