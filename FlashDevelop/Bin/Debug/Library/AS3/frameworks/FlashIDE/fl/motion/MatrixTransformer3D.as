package fl.motion
{
	import __AS3__.vec.Vector;
	import flash.geom.Vector3D;
	import flash.geom.Matrix3D;

	/**
	 * @private helper functions for supporting 3D motion in the tool. Users should use Flash Player APIs in flash.geom
	 */
	public class MatrixTransformer3D
	{
		public static const AXIS_X : int = 0;
		public static const AXIS_Y : int = 1;
		public static const AXIS_Z : int = 2;

		public static function rotateAboutAxis (radians:Number, axis:int) : Matrix3D;
		public static function getVector (mat:Matrix3D, index:int) : Vector3D;
		public static function getMatrix3D (vec0:Vector3D, vec1:Vector3D, vec2:Vector3D, vec3:Vector3D) : Matrix3D;
		public static function rotateAxis (mat:Matrix3D, radians:Number, axis:int) : Matrix3D;
		public static function normalizeVector (vec:Vector3D) : Vector3D;
		public static function getRawDataVector (mat:Matrix3D) : Vector.<Number>;
	}
}
