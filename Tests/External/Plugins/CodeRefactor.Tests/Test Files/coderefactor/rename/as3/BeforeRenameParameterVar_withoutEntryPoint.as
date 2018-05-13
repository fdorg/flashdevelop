package {
	public class Main {
		public function Main() {
		}

		// баг при ренйме - у меня хоткей ALT + R. Если начать переименовывать a или b, FD частично повиснет, дальше ренймы нигде работать не будут, пока не рестартнем
		public static function AxB(a : int, b : int) : int {
			return a * b;
		}
	}
}