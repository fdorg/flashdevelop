package com.buraks.utils {

	/*
	fastmem.as
	Visit http://www.buraks.com/azoth for more information.
	Copyright (c) 2010 Manitu Group. http://www.manitugroup.com
	*/

	import flash.utils.ByteArray;
	import flash.system.ApplicationDomain;
	
	public class fastmem {

		private static const VERSION:String="1.04";
		private static var _mem:ByteArray=null;
		private static var _memPrev:ByteArray=null;
		private static var _memSelected:Boolean=false;

		public static function fastSelectMem(mem:flash.utils.ByteArray):void {
			if (mem.length<ApplicationDomain.MIN_DOMAIN_MEMORY_LENGTH) throw new Error('fastmem: ByteArray length < Minimum Domain Memory length');
			if (_memSelected) throw new Error('fastmem: Memory already selected!');
			_memPrev=ApplicationDomain.currentDomain.domainMemory;
			_mem=mem;
			ApplicationDomain.currentDomain.domainMemory=_mem;
			_memSelected=true;
		}

		public static function fastDeselectMem():void {
			if (!_memSelected) throw new Error('fastmem: Memory not selected!');
			ApplicationDomain.currentDomain.domainMemory=_memPrev;
			_memPrev=null;
			_mem=null;
			_memSelected=false;
		}
		
		public static function fastSelectMem102(mem:flash.utils.ByteArray):void {
			if (mem.length<ApplicationDomain.MIN_DOMAIN_MEMORY_LENGTH) throw new Error('ByteArray length < Minimum Domain Memory length');
			ApplicationDomain.currentDomain.domainMemory=mem;
			_mem=ApplicationDomain.currentDomain.domainMemory;
		}

		public static function fastGetByte(address:int):int {
			return(_mem[address]);
		}

		public static function fastGetUI16(address:int):int {
			return( (_mem[address+1]<<8) | (_mem[address]) );
		}

		public static function fastGetI32(address:int):int {
			return( (_mem[address+3]<<24) | (_mem[address+2]<<16) | (_mem[address+1]<<8) | (_mem[address]) );
		}

		public static function fastGetFloat(address:int):Number {
			_mem.position=address;
			return (_mem.readFloat());
		}

		public static function fastGetDouble(address:int):Number {
			_mem.position=address;
			return (_mem.readDouble());
		}

		public static function fastSetByte(value:int,address:int):void {
			_mem[address]=value & 0xFF;
		}

		public static function fastSetI16(value:int,address:int):void {
			_mem[address+1]=(value>>8) & 0xFF;
			_mem[address]=value & 0xFF;
			
		}

		public static function fastSetI32(value:int,address:int):void {
			_mem[address+3]=(value>>24) & 0xFF;
			_mem[address+2]=(value>>16) & 0xFF;
			_mem[address+1]=(value>>8) & 0xFF;
			_mem[address]=(value) & 0xFF;
		}

		public static function fastSetFloat(value:Number,address:int):void {
			_mem.position=address;
			_mem.writeFloat(value);
		}

		public static function fastSetDouble(value:Number,address:int):void {
			_mem.position=address;
			_mem.writeDouble(value);
		}

		public static function fastSignExtend1(value:int):int {
			value = value & 0x01;
			if (value & 0x01){value=value | 0xFFFFFFFE};
			return(value);
		}

		public static function fastSignExtend8(value:int):int {
			value = value & 0xFF;
			if ((value & 0x80)!=0){value=value | 0xFFFFFF00};
			return(value);
		}

		public static function fastSignExtend16(value:int):int {
			value = value & 0xFFFF;
			if (value & 0x8000){value=value | 0xFFFF0000};
			return(value);
		}
	}
}