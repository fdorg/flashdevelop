package fl.motion
{
	/**
	 * The DynamicMatrix class calculates and stores a matrix based on given values.      * This class supports the ColorMatrixFilter and can be extended by the ColorMatrix class.     * @playerversion Flash 9     * @langversion 3.0     * @see fl.motion.ColorMatrix     * @see flash.filters.ColorMatrixFilter
	 */
	public class DynamicMatrix
	{
		/**
		 * Specifies that a matrix is prepended for concatenation.      * @playerversion Flash 9     * @langversion 3.0
		 */
		public static const MATRIX_ORDER_PREPEND : int = 0;
		/**
		 * Specifies that a matrix is appended for concatenation.      * @playerversion Flash 9     * @langversion 3.0
		 */
		public static const MATRIX_ORDER_APPEND : int = 1;
		/**
		 * @private
		 */
		protected var m_width : int;
		/**
		 * @private
		 */
		protected var m_height : int;
		/**
		 * @private
		 */
		protected var m_matrix : Array;

		/**
		 * Constructs a matrix with the given number of rows and columns.      * @param width Number of columns.     * @param height Number of rows.     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function DynamicMatrix (width:int, height:int);
		/**
		 * @private
		 */
		protected function Create (width:int, height:int) : void;
		/**
		 * @private
		 */
		protected function Destroy () : void;
		/**
		 * Returns the number of columns in the current matrix.      * @return The number of columns.     * @playerversion Flash 9     * @langversion 3.0     * @see #GetHeight
		 */
		public function GetWidth () : Number;
		/**
		 * Returns the number of rows in the current matrix.      * @return The number of rows.     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function GetHeight () : Number;
		/**
		 * Returns the value at the specified zero-based row and column in the current matrix.      * @param row The row containing the value you want.     * @param col The column containing the value you want.     * @return Number The value at the specified row and column location.     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function GetValue (row:int, col:int) : Number;
		/**
		 * Sets the value at a specified zero-based row and column in the current matrix.      * @param row The row containing the value you want to set.     * @param col The column containing the value you want to set.     * @param value The number to insert into the matrix.     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function SetValue (row:int, col:int, value:Number) : void;
		/**
		 * Sets the current matrix to an identity matrix.      * @playerversion Flash 9     * @langversion 3.0     * @see flash.geom.Matrix#identity()
		 */
		public function LoadIdentity () : void;
		/**
		 * Sets all values in the current matrix to zero.      * @playerversion Flash 9     * @langversion 3.0
		 */
		public function LoadZeros () : void;
		/**
		 * Multiplies the current matrix with a specified matrix; and either     * appends or prepends the specified matrix. Use the     * second parameter of the <code>DynamicMatrix.Multiply()</code> method to      * append or prepend the specified matrix.     * @param inMatrix The matrix to add to the current matrix.     * @param order Specifies whether to append or prepend the matrix from the     * <code>inMatrix</code> parameter; either <code>MATRIX_ORDER_APPEND</code>     * or <code>MATRIX_ORDER_PREPEND</code>.     * @return  A Boolean value indicating whether the multiplication succeeded (<code>true</code>) or      * failed (<code>false</code>). The value is <code>false</code> if either the current matrix or     * specified matrix (the <code>inMatrix</code> parameter) is null, or if the order is to append and the     * current matrix's width is not the same as the supplied matrix's height; or if the order is to prepend     * and the current matrix's height is not equal to the supplied matrix's width.	 *     * @playerversion Flash 9     * @langversion 3.0     * @see #MATRIX_ORDER_PREPEND     * @see #MATRIX_ORDER_APPEND
		 */
		public function Multiply (inMatrix:DynamicMatrix, order:int = MATRIX_ORDER_PREPEND) : Boolean;
		/**
		 * Multiplies a number with each item in the matrix and stores the results in     * the current matrix.     * @param value A number to multiply by each item in the matrix.     * @return A Boolean value indicating whether the multiplication succeeded (<code>true</code>)     * or failed (<code>false</code>).     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function MultiplyNumber (value:Number) : Boolean;
		/**
		 * Adds the current matrix with a specified matrix. The      * current matrix becomes the result of the addition (in other     * words the <code>DynamicMatrix.Add()</code> method does     * not create a new matrix to contain the result).     * @param inMatrix The matrix to add to the current matrix.     * @return A Boolean value indicating whether the addition succeeded (<code>true</code>)     * or failed (<code>false</code>). If the dimensions of the matrices are not     * the same, <code>DynamicMatrix.Add()</code> returns <code>false</code>.     * @playerversion Flash 9     * @langversion 3.0
		 */
		public function Add (inMatrix:DynamicMatrix) : Boolean;
	}
}
