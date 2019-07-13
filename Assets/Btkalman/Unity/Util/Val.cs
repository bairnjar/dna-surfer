namespace Btkalman.Unity.Util {
    public class Val<E> {
        public Val() { }
        public Val(E value) { this.Value = value; }
        public E Value;

        public override string ToString() {
            return string.Format("Val<{0}>", Value);
        }
    }
}