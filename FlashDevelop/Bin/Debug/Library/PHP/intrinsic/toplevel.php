<?
/**
* Absolute value
* @return number
* @version PHP 3, PHP 4, PHP 5
* @param $number mixed
*/
function abs($number);
/**
* Arc cosine
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function acos($arg);
/**
* Inverse hyperbolic cosine
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function acosh($arg);
/**
* Quote string with slashes in a C style
* @return string
* @version PHP 4, PHP 5
* @param $str string
* @param $charlist string
*/
function addcslashes($str, $charlist);
/**
* Quote string with slashes
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function addslashes($str);
/**
* Dynamic class and object aggregation of methods and properties
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
*/
function aggregate($object, $class_name);
/**
* Dynamic class and object aggregation of methods
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
*/
function aggregate_methods($object, $class_name);
/**
* Selective dynamic class methods aggregation to an object
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
* @param $methods_list array
* @param $exclude (optional) bool
*/
function aggregate_methods_by_list($object, $class_name, $methods_list, $exclude);
/**
* Selective class methods aggregation to an object using a regular expression
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
* @param $regexp string
* @param $exclude (optional) bool
*/
function aggregate_methods_by_regexp($object, $class_name, $regexp, $exclude);
/**
* Dynamic aggregation of class properties to an object
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
*/
function aggregate_properties($object, $class_name);
/**
* Selective dynamic class properties aggregation to an object
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
* @param $properties_list array
* @param $exclude (optional) bool
*/
function aggregate_properties_by_list($object, $class_name, $properties_list, $exclude);
/**
* Selective class properties aggregation to an object using a regular expression
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name string
* @param $regexp string
* @param $exclude (optional) bool
*/
function aggregate_properties_by_regexp($object, $class_name, $regexp, $exclude);
/**
* Alias of aggregate_info()
* @return &#13;
* @version PHP 4 >= 4.2.0
*/
function aggregation_info();
/**
* Terminate apache process after this request
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
*/
function apache_child_terminate();
/**
* Get an Apache subprocess_env variable
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $variable string
* @param $walk_to_top (optional) bool
*/
function apache_getenv($variable, $walk_to_top);
/**
* Get a list of loaded Apache modules
* @return array
* @version PHP 4 >= 4.3.2, PHP 5
*/
function apache_get_modules();
/**
* Fetch Apache version
* @return string
* @version PHP 4 >= 4.3.2, PHP 5
*/
function apache_get_version();
/**
* Perform a partial request for the specified URI and return all info about it
* @return object
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $filename string
*/
function apache_lookup_uri($filename);
/**
* Get and set apache request notes
* @return string
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $note_name string
* @param $note_value (optional) string
*/
function apache_note($note_name, $note_value);
/**
* Fetch all HTTP request headers
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function apache_request_headers();
/**
* Fetch all HTTP response headers
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function apache_response_headers();
/**
* Set an Apache subprocess_env variable
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $variable string
* @param $value string
* @param $walk_to_top (optional) bool
*/
function apache_setenv($variable, $value, $walk_to_top);
/**
* Retrieves cached information (and meta-data) from APC's data store
* @return array
* @version PECL
* @param $cache_type string
*/
function apc_cache_info($cache_type);
/**
* Clears the APC cache
* @return bool
* @version PECL
* @param $cache_type string
*/
function apc_clear_cache($cache_type);
/**
* Defines a set of constants for later retrieval and mass-definition
* @return bool
* @version PECL
* @param $key string
* @param $constants array
* @param $case_sensitive (optional) bool
*/
function apc_define_constants($key, $constants, $case_sensitive);
/**
* Removes a stored variable from the cache
* @return bool
* @version PECL
* @param $key string
*/
function apc_delete($key);
/**
* Fetch a stored variable from the cache
* @return mixed
* @version PECL
* @param $key string
*/
function apc_fetch($key);
/**
* Loads a set of constants from the cache
* @return bool
* @version PECL
* @param $key string
* @param $case_sensitive (optional) bool
*/
function apc_load_constants($key, $case_sensitive);
/**
* Retrieves APC's Shared Memory Allocation information
* @return array
* @version PECL
*/
function apc_sma_info();
/**
* Cache a variable in the data store
* @return bool
* @version PECL
* @param $key string
* @param $var mixed
* @param $ttl (optional) int
*/
function apc_store($key, $var, $ttl);
/**
* Stops the interpreter and waits on a CR from the socket
* @return bool
* @version PECL
* @param $debug_level int
*/
function apd_breakpoint($debug_level);
/**
* Restarts the interpreter
* @return bool
* @version PECL
* @param $debug_level int
*/
function apd_continue($debug_level);
/**
* Echo to the debugging socket
* @return bool
* @version PECL
* @param $output string
*/
function apd_echo($output);
/**
* Starts the session debugging
* @return 
* @version PECL
* @param $dump_directory string
*/
function apd_set_pprof_trace($dump_directory);
/**
* Create an array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $params1 mixed
*/
function array($params1);
/**
* Returns an array with all string keys lowercased or uppercased
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $input array
* @param $case (optional) int
*/
function array_change_key_case($input, $case);
/**
* Split an array into chunks
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $input array
* @param $size int
* @param $preserve_keys (optional) bool
*/
function array_chunk($input, $size, $preserve_keys);
/**
* Creates an array by using one array for keys and another for its values
* @return array
* @version PHP 5
* @param $keys array
* @param $values array
*/
function array_combine($keys, $values);
/**
* Counts all the values of an array
* @return array
* @version PHP 4, PHP 5
* @param $input array
*/
function array_count_values($input);
/**
* Computes the difference of arrays
* @return array
* @version PHP 4 >= 4.0.1, PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_diff($array1, $array2, $params1);
/**
* Computes the difference of arrays with additional index check
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_diff_assoc($array1, $array2, $params1);
/**
* Computes the difference of arrays using keys for comparison
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_diff_key($array1, $array2, $params1);
/**
* Computes the difference of arrays with additional index check which is performed by a user supplied callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $key_compare_func (optional) callback
*/
function array_diff_uassoc($array1, $array2, $params1, $key_compare_func);
/**
* Computes the difference of arrays using a callback function on the keys for comparison
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $key_compare_func (optional) callback
*/
function array_diff_ukey($array1, $array2, $params1, $key_compare_func);
/**
* Fill an array with values
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $start_index int
* @param $num int
* @param $value mixed
*/
function array_fill($start_index, $num, $value);
/**
* Filters elements of an array using a callback function
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $input array
* @param $callback (optional) callback
*/
function array_filter($input, $callback);
/**
* Exchanges all keys with their associated values in an array
* @return array
* @version PHP 4, PHP 5
* @param $trans array
*/
function array_flip($trans);
/**
* Computes the intersection of arrays
* @return array
* @version PHP 4 >= 4.0.1, PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_intersect($array1, $array2, $params1);
/**
* Computes the intersection of arrays with additional index check
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_intersect_assoc($array1, $array2, $params1);
/**
* Computes the intersection of arrays using keys for comparison
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
*/
function array_intersect_key($array1, $array2, $params1);
/**
* Computes the intersection of arrays with additional index check, compares indexes by a callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $key_compare_func (optional) callback
*/
function array_intersect_uassoc($array1, $array2, $params1, $key_compare_func);
/**
* Computes the intersection of arrays using a callback function on the keys for comparison
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $key_compare_func (optional) callback
*/
function array_intersect_ukey($array1, $array2, $params1, $key_compare_func);
/**
* Return all the keys of an array
* @return array
* @version PHP 4, PHP 5
* @param $input array
* @param $search_value (optional) mixed
* @param $strict (optional) bool
*/
function array_keys($input, $search_value, $strict);
/**
* Checks if the given key or index exists in the array
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $key mixed
* @param $search array
*/
function array_key_exists($key, $search);
/**
* Applies the callback to the elements of the given arrays
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $callback callback
* @param $arr1 array
* @param $params1 (optional) array
*/
function array_map($callback, $arr1, $params1);
/**
* Merge one or more arrays
* @return array
* @version PHP 4, PHP 5
* @param $array1 array
* @param $array2 (optional) array
* @param $params1 (optional) array
*/
function array_merge($array1, $array2, $params1);
/**
* Merge two or more arrays recursively
* @return array
* @version PHP 4 >= 4.0.1, PHP 5
* @param $array1 array
* @param $params1 (optional) array
*/
function array_merge_recursive($array1, $params1);
/**
* Sort multiple or multi-dimensional arrays
* @return bool
* @version PHP 4, PHP 5
* @param $ar1 array
* @param $arg (optional) mixed
* @param $params1 (optional) mixed
* @param $params2 (optional) array
*/
function array_multisort($ar1, $arg, $params1, $params2);
/**
* Pad array to the specified length with a value
* @return array
* @version PHP 4, PHP 5
* @param $input array
* @param $pad_size int
* @param $pad_value mixed
*/
function array_pad($input, $pad_size, $pad_value);
/**
* Pop the element off the end of array
* @return mixed
* @version PHP 4, PHP 5
* @param &$array array
*/
function array_pop(&$array);
/**
* Calculate the product of values in an array
* @return number
* @version PHP 5 >= 5.1.0RC1
* @param $array array
*/
function array_product($array);
/**
* Push one or more elements onto the end of array
* @return int
* @version PHP 4, PHP 5
* @param &$array array
* @param $var mixed
* @param $params1 (optional) mixed
*/
function array_push(&$array, $var, $params1);
/**
* Pick one or more random entries out of an array
* @return mixed
* @version PHP 4, PHP 5
* @param $input array
* @param $num_req (optional) int
*/
function array_rand($input, $num_req);
/**
* Iteratively reduce the array to a single value using a callback function
* @return mixed
* @version PHP 4 >= 4.0.5, PHP 5
* @param $input array
* @param $function callback
* @param $initial (optional) int
*/
function array_reduce($input, $function, $initial);
/**
* Return an array with elements in reverse order
* @return array
* @version PHP 4, PHP 5
* @param $array array
* @param $preserve_keys (optional) bool
*/
function array_reverse($array, $preserve_keys);
/**
* Searches the array for a given value and returns the corresponding key if successful
* @return mixed
* @version PHP 4 >= 4.0.5, PHP 5
* @param $needle mixed
* @param $haystack array
* @param $strict (optional) bool
*/
function array_search($needle, $haystack, $strict);
/**
* Shift an element off the beginning of array
* @return mixed
* @version PHP 4, PHP 5
* @param &$array array
*/
function array_shift(&$array);
/**
* Extract a slice of the array
* @return array
* @version PHP 4, PHP 5
* @param $array array
* @param $offset int
* @param $length (optional) int
* @param $preserve_keys (optional) bool
*/
function array_slice($array, $offset, $length, $preserve_keys);
/**
* Remove a portion of the array and replace it with something else
* @return array
* @version PHP 4, PHP 5
* @param &$input array
* @param $offset int
* @param $length (optional) int
* @param $replacement (optional) array
*/
function array_splice(&$input, $offset, $length, $replacement);
/**
* Calculate the sum of values in an array
* @return number
* @version PHP 4 >= 4.0.4, PHP 5
* @param $array array
*/
function array_sum($array);
/**
* Computes the difference of arrays by using a callback function for data comparison
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
*/
function array_udiff($array1, $array2, $params1, $data_compare_func);
/**
* Computes the difference of arrays with additional index check, compares data by a callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
*/
function array_udiff_assoc($array1, $array2, $params1, $data_compare_func);
/**
* Computes the difference of arrays with additional index check, compares data and indexes by a callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
* @param $key_compare_func (optional) callback
*/
function array_udiff_uassoc($array1, $array2, $params1, $data_compare_func, $key_compare_func);
/**
* Computes the intersection of arrays, compares data by a callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
*/
function array_uintersect($array1, $array2, $params1, $data_compare_func);
/**
* Computes the intersection of arrays with additional index check, compares data by a callback function
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
*/
function array_uintersect_assoc($array1, $array2, $params1, $data_compare_func);
/**
* Computes the intersection of arrays with additional index check, compares data and indexes by a callback functions
* @return array
* @version PHP 5
* @param $array1 array
* @param $array2 array
* @param $params1 (optional) array
* @param $data_compare_func (optional) callback
* @param $key_compare_func (optional) callback
*/
function array_uintersect_uassoc($array1, $array2, $params1, $data_compare_func, $key_compare_func);
/**
* Removes duplicate values from an array
* @return array
* @version PHP 4 >= 4.0.1, PHP 5
* @param $array array
*/
function array_unique($array);
/**
* Prepend one or more elements to the beginning of an array
* @return int
* @version PHP 4, PHP 5
* @param &$array array
* @param $var mixed
* @param $params1 (optional) mixed
*/
function array_unshift(&$array, $var, $params1);
/**
* Return all the values of an array
* @return array
* @version PHP 4, PHP 5
* @param $input array
*/
function array_values($input);
/**
* Apply a user function to every member of an array
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param &$array array
* @param $funcname callback
* @param $userdata (optional) mixed
*/
function array_walk(&$array, $funcname, $userdata);
/**
* Apply a user function recursively to every member of an array
* @return bool
* @version PHP 5
* @param &$input array
* @param $funcname callback
* @param $userdata (optional) mixed
*/
function array_walk_recursive(&$input, $funcname, $userdata);
/**
* Sort an array in reverse order and maintain index association
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function arsort(&$array, $sort_flags);
/**
* Translate string from ASCII to EBCDIC
* @return int
* @version PHP 3 >= 3.0.17
* @param $ascii_str string
*/
function ascii2ebcdic($ascii_str);
/**
* Arc sine
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function asin($arg);
/**
* Inverse hyperbolic sine
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function asinh($arg);
/**
* Sort an array and maintain index association
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function asort(&$array, $sort_flags);
/**
* Check a word [deprecated]
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4 <= 4.2.3
* @param $dictionary_link int
* @param $word string
*/
function aspell_check($dictionary_link, $word);
/**
* Check a word without changing its case or trying to trim it [deprecated]
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4 <= 4.2.3
* @param $dictionary_link int
* @param $word string
*/
function aspell_check_raw($dictionary_link, $word);
/**
* Load a new dictionary [deprecated]
* @return int
* @version PHP 3 >= 3.0.7, PHP 4 <= 4.2.3
* @param $master string
* @param $personal (optional) string
*/
function aspell_new($master, $personal);
/**
* Suggest spellings of a word [deprecated]
* @return array
* @version PHP 3 >= 3.0.7, PHP 4 <= 4.2.3
* @param $dictionary_link int
* @param $word string
*/
function aspell_suggest($dictionary_link, $word);
/**
* Checks if assertion is FALSE
* @return bool
* @version PHP 4, PHP 5
* @param $assertion mixed
*/
function assert($assertion);
/**
* Set/get the various assert flags
* @return mixed
* @version PHP 4, PHP 5
* @param $what int
* @param $value (optional) mixed
*/
function assert_options($what, $value);
/**
* Arc tangent
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function atan($arg);
/**
* Arc tangent of two variables
* @return float
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
* @param $y float
* @param $x float
*/
function atan2($y, $x);
/**
* Inverse hyperbolic tangent
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function atanh($arg);
/**
* Decodes data encoded with MIME base64
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $encoded_data string
*/
function base64_decode($encoded_data);
/**
* Encodes data with MIME base64
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $data string
*/
function base64_encode($data);
/**
* Returns filename component of path
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $path string
* @param $suffix (optional) string
*/
function basename($path, $suffix);
/**
* Convert a number between arbitrary bases
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $number string
* @param $frombase int
* @param $tobase int
*/
function base_convert($number, $frombase, $tobase);
/**
* Add two arbitrary precision numbers
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $right_operand string
* @param $scale (optional) int
*/
function bcadd($left_operand, $right_operand, $scale);
/**
* Compare two arbitrary precision numbers
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $right_operand string
* @param $scale (optional) int
*/
function bccomp($left_operand, $right_operand, $scale);
/**
* Divide two arbitrary precision numbers
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $right_operand string
* @param $scale (optional) int
*/
function bcdiv($left_operand, $right_operand, $scale);
/**
* Get modulus of an arbitrary precision number
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $modulus string
*/
function bcmod($left_operand, $modulus);
/**
* Multiply two arbitrary precision number
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $right_operand string
* @param $scale (optional) int
*/
function bcmul($left_operand, $right_operand, $scale);
/**
* Reads and creates classes from a bz compressed file
* @return bool
* @version PECL
* @param $filename string
*/
function bcompiler_load($filename);
/**
* Reads and creates classes from a bcompiler exe file
* @return bool
* @version PECL
* @param $filename string
*/
function bcompiler_load_exe($filename);
/**
* Reads the bytecodes of a class and calls back to a user function
* @return bool
* @version PECL
* @param $class string
* @param $callback string
*/
function bcompiler_parse_class($class, $callback);
/**
* Reads and creates classes from a filehandle
* @return bool
* @version PECL
* @param $filehandle resource
*/
function bcompiler_read($filehandle);
/**
* Writes an defined class as bytecodes
* @return bool
* @version PECL
* @param $filehandle resource
* @param $className string
* @param $extends (optional) string
*/
function bcompiler_write_class($filehandle, $className, $extends);
/**
* Writes a defined constant as bytecodes
* @return bool
* @version PECL
* @param $filehandle resource
* @param $constantName string
*/
function bcompiler_write_constant($filehandle, $constantName);
/**
* Writes the start pos, and sig to the end of a exe type file
* @return bool
* @version PECL
* @param $filehandle resource
* @param $startpos int
*/
function bcompiler_write_exe_footer($filehandle, $startpos);
/**
* Writes a php source file as bytecodes
* @return bool
* @version PECL
* @param $filehandle resource
* @param $filename string
*/
function bcompiler_write_file($filehandle, $filename);
/**
* Writes the single character x00 to indicate End of compiled data
* @return bool
* @version PECL
* @param $filehandle resource
*/
function bcompiler_write_footer($filehandle);
/**
* Writes an defined function as bytecodes
* @return bool
* @version PECL
* @param $filehandle resource
* @param $functionName string
*/
function bcompiler_write_function($filehandle, $functionName);
/**
* Writes all functions defined in a file as bytecodes
* @return bool
* @version PECL
* @param $filehandle resource
* @param $fileName string
*/
function bcompiler_write_functions_from_file($filehandle, $fileName);
/**
* Writes the bcompiler header
* @return bool
* @version PECL
* @param $filehandle resource
* @param $write_ver (optional) string
*/
function bcompiler_write_header($filehandle, $write_ver);
/**
* Raise an arbitrary precision number to another
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $x string
* @param $y string
* @param $scale (optional) int
*/
function bcpow($x, $y, $scale);
/**
* Raise an arbitrary precision number to another, reduced by a specified modulus
* @return string
* @version PHP 5
* @param $x string
* @param $y string
* @param $modulus string
* @param $scale (optional) int
*/
function bcpowmod($x, $y, $modulus, $scale);
/**
* Set default scale parameter for all bc math functions
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $scale int
*/
function bcscale($scale);
/**
* Get the square root of an arbitrary precision number
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $operand string
* @param $scale (optional) int
*/
function bcsqrt($operand, $scale);
/**
* Subtract one arbitrary precision number from another
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $left_operand string
* @param $right_operand string
* @param $scale (optional) int
*/
function bcsub($left_operand, $right_operand, $scale);
/**
* Convert binary data into hexadecimal representation
* @return string
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $str string
*/
function bin2hex($str);
/**
* Binary to decimal
* @return number
* @version PHP 3, PHP 4, PHP 5
* @param $binary_string string
*/
function bindec($binary_string);
/**
* Sets the path for a domain
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $domain string
* @param $directory string
*/
function bindtextdomain($domain, $directory);
/**
* Specify the character encoding in which the messages from the DOMAIN message catalog will be returned
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $domain string
* @param $codeset string
*/
function bind_textdomain_codeset($domain, $codeset);
/**
* Close a bzip2 file
* @return int
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
*/
function bzclose($bz);
/**
* Compress a string into bzip2 encoded data
* @return mixed
* @version PHP 4 >= 4.3.3, PHP 5
* @param $source string
* @param $blocksize (optional) int
* @param $workfactor (optional) int
*/
function bzcompress($source, $blocksize, $workfactor);
/**
* Decompresses bzip2 encoded data
* @return mixed
* @version PHP 4 >= 4.3.3, PHP 5
* @param $source string
* @param $small (optional) int
*/
function bzdecompress($source, $small);
/**
* Returns a bzip2 error number
* @return int
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
*/
function bzerrno($bz);
/**
* Returns the bzip2 error number and error string in an array
* @return array
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
*/
function bzerror($bz);
/**
* Returns a bzip2 error string
* @return string
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
*/
function bzerrstr($bz);
/**
* Force a write of all buffered data
* @return int
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
*/
function bzflush($bz);
/**
* Opens a bzip2 compressed file
* @return resource
* @version PHP 4 >= 4.3.3, PHP 5
* @param $filename string
* @param $mode string
*/
function bzopen($filename, $mode);
/**
* Binary safe bzip2 file read
* @return string
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
* @param $length (optional) int
*/
function bzread($bz, $length);
/**
* Binary safe bzip2 file write
* @return int
* @version PHP 4 >= 4.3.3, PHP 5
* @param $bz resource
* @param $data string
* @param $length (optional) int
*/
function bzwrite($bz, $data, $length);
/**
* Call a user function given by the first parameter
* @return mixed
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $function callback
* @param $parameter (optional) mixed
* @param $params1 (optional) mixed
*/
function call_user_func($function, $parameter, $params1);
/**
* Call a user function given with an array of parameters
* @return mixed
* @version PHP 4 >= 4.0.4, PHP 5
* @param $function callback
* @param $param_arr array
*/
function call_user_func_array($function, $param_arr);
/**
* Call a user method on an specific object [deprecated]
* @return mixed
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $method_name string
* @param &$obj object
* @param $parameter (optional) mixed
* @param $params1 (optional) mixed
*/
function call_user_method($method_name, &$obj, $parameter, $params1);
/**
* Call a user method given with an array of parameters [deprecated]
* @return mixed
* @version PHP 4 >= 4.0.5, PHP 5
* @param $method_name string
* @param &$obj object
* @param $paramarr array
*/
function call_user_method_array($method_name, &$obj, $paramarr);
/**
* Return the number of days in a month for a given year and calendar
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $calendar int
* @param $month int
* @param $year int
*/
function cal_days_in_month($calendar, $month, $year);
/**
* Converts from Julian Day Count to a supported calendar
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $jd int
* @param $calendar int
*/
function cal_from_jd($jd, $calendar);
/**
* Returns information about a particular calendar
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $calendar int
*/
function cal_info($calendar);
/**
* Converts from a supported calendar to Julian Day Count
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $calendar int
* @param $month int
* @param $day int
* @param $year int
*/
function cal_to_jd($calendar, $month, $day, $year);
/**
* Add data to a transaction
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
* @param $argtype string
* @param $argval string
*/
function ccvs_add($session, $invoice, $argtype, $argval);
/**
* Perform credit authorization test on a transaction
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_auth($session, $invoice);
/**
* Performs a command which is peculiar to a single protocol, and thus is not available in the general CCVS API
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $type string
* @param $argval string
*/
function ccvs_command($session, $type, $argval);
/**
* Find out how many transactions of a given type are stored in the system
* @return int
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $type string
*/
function ccvs_count($session, $type);
/**
* Delete a transaction
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_delete($session, $invoice);
/**
* Terminate CCVS engine and do cleanup work
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $sess string
*/
function ccvs_done($sess);
/**
* Initialize CCVS for use
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $name string
*/
function ccvs_init($name);
/**
* Look up an item of a particular type in the database #
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
* @param $inum int
*/
function ccvs_lookup($session, $invoice, $inum);
/**
* Create a new, blank transaction
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_new($session, $invoice);
/**
* Return the status of the background communication process
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $type string
*/
function ccvs_report($session, $type);
/**
* Transfer funds from the merchant to the credit card holder
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_return($session, $invoice);
/**
* Perform a full reversal on an already-processed authorization
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_reverse($session, $invoice);
/**
* Transfer funds from the credit card holder to the merchant
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_sale($session, $invoice);
/**
* Check the status of an invoice
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_status($session, $invoice);
/**
* Get text return value for previous function call
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
*/
function ccvs_textvalue($session);
/**
* Perform a full reversal on a completed transaction
* @return string
* @version 4.0.2 - 4.2.3 only
* @param $session string
* @param $invoice string
*/
function ccvs_void($session, $invoice);
/**
* Round fractions up
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $value float
*/
function ceil($value);
/**
* Change directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $directory string
*/
function chdir($directory);
/**
* Validate a Gregorian date
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $month int
* @param $day int
* @param $year int
*/
function checkdate($month, $day, $year);
/**
* Check DNS records corresponding to a given Internet host name or IP address
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $host string
* @param $type (optional) string
*/
function checkdnsrr($host, $type);
/**
* Changes file group
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $group mixed
*/
function chgrp($filename, $group);
/**
* Changes file mode
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $mode int
*/
function chmod($filename, $mode);
/**
* Alias of rtrim()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function chop();
/**
* Changes file owner
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $user mixed
*/
function chown($filename, $user);
/**
* Return a specific character
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $ascii int
*/
function chr($ascii);
/**
* Change the root directory
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $directory string
*/
function chroot($directory);
/**
* Split a string into smaller chunks
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $body string
* @param $chunklen (optional) int
* @param $end (optional) string
*/
function chunk_split($body, $chunklen, $end);
/**
* Import new class method definitions from a file
* @return array
* @version PECL
* @param $filename string
*/
function classkit_import($filename);
/**
* Dynamically adds a new method to a given class
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $args string
* @param $code string
* @param $flags (optional) int
*/
function classkit_method_add($classname, $methodname, $args, $code, $flags);
/**
* Copies a method from class to another
* @return bool
* @version PECL
* @param $dClass string
* @param $dMethod string
* @param $sClass string
* @param $sMethod (optional) string
*/
function classkit_method_copy($dClass, $dMethod, $sClass, $sMethod);
/**
* Dynamically changes the code of the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $args string
* @param $code string
* @param $flags (optional) int
*/
function classkit_method_redefine($classname, $methodname, $args, $code, $flags);
/**
* Dynamically removes the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
*/
function classkit_method_remove($classname, $methodname);
/**
* Dynamically changes the name of the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $newname string
*/
function classkit_method_rename($classname, $methodname, $newname);
/**
* Checks if the class has been defined
* @return bool
* @version PHP 4, PHP 5
* @param $class_name string
* @param $autoload (optional) bool
*/
function class_exists($class_name, $autoload);
/**
* Return the interfaces which are implemented by the given class
* @return array
* @version PHP 5
* @param $class mixed
* @param $autoload (optional) bool
*/
function class_implements($class, $autoload);
/**
* Return the parent classes of the given class
* @return array
* @version PHP 5
* @param $class mixed
* @param $autoload (optional) bool
*/
function class_parents($class, $autoload);
/**
* Clears file status cache
* @return 
* @version PHP 3, PHP 4, PHP 5
*/
function clearstatcache();
/**
* Close directory handle
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $dir_handle resource
*/
function closedir($dir_handle);
/**
* Close connection to system logger
* @return bool
* @version PHP 3, PHP 4, PHP 5
*/
function closelog();
/**
* Create array containing variables and their values
* @return array
* @version PHP 4, PHP 5
* @param $varname mixed
* @param $params1 (optional) mixed
*/
function compact($varname, $params1);
/**
* Increases the components reference counter [deprecated]
* @return 
* @version PHP 4 >= 4.1.0
*/
function com_addref();
/**
* Generate a globally unique identifier (GUID)
* @return string
* @version PHP 5
*/
function com_create_guid();
/**
* Connect events from a COM object to a PHP object
* @return bool
* @version PHP 4 >= 4.2.3, PHP 5
* @param $comobject variant
* @param $sinkobject object
* @param $sinkinterface (optional) mixed
*/
function com_event_sink($comobject, $sinkobject, $sinkinterface);
/**
* Gets the value of a COM Component's property [deprecated]
* @return mixed
* @version PHP 3 >= 3.0.3, PHP 4
* @param $com_object resource
* @param $property string
*/
function com_get($com_object, $property);
/**
* Returns a handle to an already running instance of a COM object
* @return variant
* @version PHP 5
* @param $progid string
* @param $code_page (optional) int
*/
function com_get_active_object($progid, $code_page);
/**
* Calls a COM component's method [deprecated]
* @return mixed
* @version PHP 4
* @param $com_object resource
* @param $function_name string
* @param $function_parameters (optional) mixed
*/
function com_invoke($com_object, $function_name, $function_parameters);
/**
* Indicates if a COM object has an IEnumVariant interface for iteration [deprecated]
* @return bool
* @version PHP 4 >= 4.1.0
* @param $com_module variant
*/
function com_isenum($com_module);
/**
* Creates a new reference to a COM component [deprecated]
* @return resource
* @version PHP 4
* @param $module_name string
* @param $server_name (optional) string
* @param $codepage (optional) int
*/
function com_load($module_name, $server_name, $codepage);
/**
* Loads a Typelib
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $typelib_name string
* @param $case_insensitive (optional) bool
*/
function com_load_typelib($typelib_name, $case_insensitive);
/**
* Process COM messages, sleeping for up to timeoutms milliseconds
* @return bool
* @version PHP 4 >= 4.2.3, PHP 5
* @param $timeoutms int
*/
function com_message_pump($timeoutms);
/**
* Print out a PHP class definition for a dispatchable interface
* @return bool
* @version PHP 4 >= 4.2.3, PHP 5
* @param $comobject object
* @param $dispinterface (optional) string
* @param $wantsink (optional) bool
*/
function com_print_typeinfo($comobject, $dispinterface, $wantsink);
/**
* Alias of com_get()
* @return &#13;
* @version PHP 3 >= 3.0.3, PHP 4
*/
function com_propget();
/**
* Alias of com_set()
* @return &#13;
* @version PHP 3 >= 3.0.3, PHP 4
*/
function com_propput();
/**
* Alias of com_set()
* @return &#13;
* @version PHP 3 >= 3.0.3, PHP 4
*/
function com_propset();
/**
* Decreases the components reference counter [deprecated]
* @return 
* @version PHP 4 >= 4.1.0
*/
function com_release();
/**
* Assigns a value to a COM component's property
* @return 
* @version PHP 3 >= 3.0.3, PHP 4
* @param $com_object resource
* @param $property string
* @param $value mixed
*/
function com_set($com_object, $property, $value);
/**
* Returns TRUE if client disconnected
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function connection_aborted();
/**
* Returns connection status bitfield
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function connection_status();
/**
* Return TRUE if script timed out
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4 <= 4.0.4
*/
function connection_timeout();
/**
* Returns the value of a constant
* @return mixed
* @version PHP 4 >= 4.0.4, PHP 5
* @param $name string
*/
function constant($name);
/**
* Convert from one Cyrillic character set to another
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $str string
* @param $from string
* @param $to string
*/
function convert_cyr_string($str, $from, $to);
/**
* Decode a uuencoded string
* @return string
* @version PHP 5
* @param $data string
*/
function convert_uudecode($data);
/**
* Uuencode a string
* @return string
* @version PHP 5
* @param $data string
*/
function convert_uuencode($data);
/**
* Copies file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $source string
* @param $dest string
*/
function copy($source, $dest);
/**
* Cosine
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function cos($arg);
/**
* Hyperbolic cosine
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function cosh($arg);
/**
* Count elements in an array, or properties in an object
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
* @param $mode (optional) int
*/
function count($var, $mode);
/**
* Return information about characters used in a string
* @return mixed
* @version PHP 4, PHP 5
* @param $string string
* @param $mode (optional) int
*/
function count_chars($string, $mode);
/**
* Adds annotation
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $title string
* @param $content string
* @param $mode (optional) int
*/
function cpdf_add_annotation($pdf_document, $llx, $lly, $urx, $ury, $title, $content, $mode);
/**
* Adds bookmark for current page
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $lastoutline int
* @param $sublevel int
* @param $open int
* @param $pagenr int
* @param $text string
*/
function cpdf_add_outline($pdf_document, $lastoutline, $sublevel, $open, $pagenr, $text);
/**
* Draws an arc
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $radius float
* @param $start float
* @param $end float
* @param $mode (optional) int
*/
function cpdf_arc($pdf_document, $x_coor, $y_coor, $radius, $start, $end, $mode);
/**
* Starts text section
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_begin_text($pdf_document);
/**
* Draw a circle
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $radius float
* @param $mode (optional) int
*/
function cpdf_circle($pdf_document, $x_coor, $y_coor, $radius, $mode);
/**
* Clips to current path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_clip($pdf_document);
/**
* Closes the pdf document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_close($pdf_document);
/**
* Close path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_closepath($pdf_document);
/**
* Close, fill and stroke current path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_closepath_fill_stroke($pdf_document);
/**
* Close path and draw line along path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_closepath_stroke($pdf_document);
/**
* Output text in next line
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $text string
*/
function cpdf_continue_text($pdf_document, $text);
/**
* Draws a curve
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $x3 float
* @param $y3 float
* @param $mode (optional) int
*/
function cpdf_curveto($pdf_document, $x1, $y1, $x2, $y2, $x3, $y3, $mode);
/**
* Ends text section
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_end_text($pdf_document);
/**
* Fill current path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_fill($pdf_document);
/**
* Fill and stroke current path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_fill_stroke($pdf_document);
/**
* Ends document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_finalize($pdf_document);
/**
* Ends page
* @return bool
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $page_number int
*/
function cpdf_finalize_page($pdf_document, $page_number);
/**
* Sets document limits for any pdf document
* @return bool
* @version PHP 4, PHP 5 <= 5.0.4
* @param $maxpages int
* @param $maxfonts int
* @param $maximages int
* @param $maxannotations int
* @param $maxobjects int
*/
function cpdf_global_set_document_limits($maxpages, $maxfonts, $maximages, $maxannotations, $maxobjects);
/**
* Opens a JPEG image
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $file_name string
* @param $x_coor float
* @param $y_coor float
* @param $angle float
* @param $width float
* @param $height float
* @param $x_scale float
* @param $y_scale float
* @param $gsave int
* @param $mode (optional) int
*/
function cpdf_import_jpeg($pdf_document, $file_name, $x_coor, $y_coor, $angle, $width, $height, $x_scale, $y_scale, $gsave, $mode);
/**
* Draws a line
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_lineto($pdf_document, $x_coor, $y_coor, $mode);
/**
* Sets current point
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_moveto($pdf_document, $x_coor, $y_coor, $mode);
/**
* Starts a new path
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_newpath($pdf_document);
/**
* Opens a new pdf document
* @return int
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $compression int
* @param $filename (optional) string
* @param $doc_limits (optional) array
*/
function cpdf_open($compression, $filename, $doc_limits);
/**
* Outputs the pdf document in memory buffer
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_output_buffer($pdf_document);
/**
* Starts new page
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $page_number int
* @param $orientation int
* @param $height float
* @param $width float
* @param $unit (optional) float
*/
function cpdf_page_init($pdf_document, $page_number, $orientation, $height, $width, $unit);
/**
* Places an image on the page
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $image int
* @param $x_coor float
* @param $y_coor float
* @param $angle float
* @param $width float
* @param $height float
* @param $gsave int
* @param $mode (optional) int
*/
function cpdf_place_inline_image($pdf_document, $image, $x_coor, $y_coor, $angle, $width, $height, $gsave, $mode);
/**
* Draw a rectangle
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $width float
* @param $height float
* @param $mode (optional) int
*/
function cpdf_rect($pdf_document, $x_coor, $y_coor, $width, $height, $mode);
/**
* Restores formerly saved environment
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_restore($pdf_document);
/**
* Draws a line
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_rlineto($pdf_document, $x_coor, $y_coor, $mode);
/**
* Sets current point
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_rmoveto($pdf_document, $x_coor, $y_coor, $mode);
/**
* Sets rotation
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $angle float
*/
function cpdf_rotate($pdf_document, $angle);
/**
* Sets text rotation angle
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdfdoc int
* @param $angle float
*/
function cpdf_rotate_text($pdfdoc, $angle);
/**
* Saves current environment
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_save($pdf_document);
/**
* Writes the pdf document into a file
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $filename string
*/
function cpdf_save_to_file($pdf_document, $filename);
/**
* Sets scaling
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_scale float
* @param $y_scale float
*/
function cpdf_scale($pdf_document, $x_scale, $y_scale);
/**
* Sets dash pattern
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $white float
* @param $black float
*/
function cpdf_setdash($pdf_document, $white, $black);
/**
* Sets flatness
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value float
*/
function cpdf_setflat($pdf_document, $value);
/**
* Sets drawing and filling color to gray value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $gray_value float
*/
function cpdf_setgray($pdf_document, $gray_value);
/**
* Sets filling color to gray value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value float
*/
function cpdf_setgray_fill($pdf_document, $value);
/**
* Sets drawing color to gray value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $gray_value float
*/
function cpdf_setgray_stroke($pdf_document, $gray_value);
/**
* Sets linecap parameter
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value int
*/
function cpdf_setlinecap($pdf_document, $value);
/**
* Sets linejoin parameter
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value int
*/
function cpdf_setlinejoin($pdf_document, $value);
/**
* Sets line width
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $width float
*/
function cpdf_setlinewidth($pdf_document, $width);
/**
* Sets miter limit
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value float
*/
function cpdf_setmiterlimit($pdf_document, $value);
/**
* Sets drawing and filling color to rgb color value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $red_value float
* @param $green_value float
* @param $blue_value float
*/
function cpdf_setrgbcolor($pdf_document, $red_value, $green_value, $blue_value);
/**
* Sets filling color to rgb color value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $red_value float
* @param $green_value float
* @param $blue_value float
*/
function cpdf_setrgbcolor_fill($pdf_document, $red_value, $green_value, $blue_value);
/**
* Sets drawing color to rgb color value
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $red_value float
* @param $green_value float
* @param $blue_value float
*/
function cpdf_setrgbcolor_stroke($pdf_document, $red_value, $green_value, $blue_value);
/**
* Sets hyperlink
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdfdoc int
* @param $xll float
* @param $yll float
* @param $xur float
* @param $xur float
* @param $url string
* @param $mode (optional) int
*/
function cpdf_set_action_url($pdfdoc, $xll, $yll, $xur, $xur, $url, $mode);
/**
* Sets character spacing
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $space float
*/
function cpdf_set_char_spacing($pdf_document, $space);
/**
* Sets the creator field in the pdf document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $creator string
*/
function cpdf_set_creator($pdf_document, $creator);
/**
* Sets current page
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $page_number int
*/
function cpdf_set_current_page($pdf_document, $page_number);
/**
* Select the current font face and size
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $font_name string
* @param $size float
* @param $encoding string
*/
function cpdf_set_font($pdf_document, $font_name, $size, $encoding);
/**
* Sets directories to search when using external fonts
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $pdfdoc int
* @param $pfmdir string
* @param $pfbdir string
*/
function cpdf_set_font_directories($pdfdoc, $pfmdir, $pfbdir);
/**
* Sets fontname to filename translation map when using external fonts
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $pdfdoc int
* @param $filename string
*/
function cpdf_set_font_map_file($pdfdoc, $filename);
/**
* Sets horizontal scaling of text
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $scale float
*/
function cpdf_set_horiz_scaling($pdf_document, $scale);
/**
* Sets the keywords field of the pdf document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $keywords string
*/
function cpdf_set_keywords($pdf_document, $keywords);
/**
* Sets distance between text lines
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $distance float
*/
function cpdf_set_leading($pdf_document, $distance);
/**
* Sets duration between pages
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $transition int
* @param $duration float
* @param $direction float
* @param $orientation int
* @param $inout int
*/
function cpdf_set_page_animation($pdf_document, $transition, $duration, $direction, $orientation, $inout);
/**
* Sets the subject field of the pdf document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $subject string
*/
function cpdf_set_subject($pdf_document, $subject);
/**
* Sets the text matrix
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $matrix array
*/
function cpdf_set_text_matrix($pdf_document, $matrix);
/**
* Sets text position
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_set_text_pos($pdf_document, $x_coor, $y_coor, $mode);
/**
* Determines how text is rendered
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $rendermode int
*/
function cpdf_set_text_rendering($pdf_document, $rendermode);
/**
* Sets the text rise
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $value float
*/
function cpdf_set_text_rise($pdf_document, $value);
/**
* Sets the title field of the pdf document
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $title string
*/
function cpdf_set_title($pdf_document, $title);
/**
* How to show the document in the viewer
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5 <= 5.0.4
* @param $pdfdoc int
* @param $preferences array
*/
function cpdf_set_viewer_preferences($pdfdoc, $preferences);
/**
* Sets spacing between words
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $space float
*/
function cpdf_set_word_spacing($pdf_document, $space);
/**
* Output text at current position
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $text string
*/
function cpdf_show($pdf_document, $text);
/**
* Output text at position
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $text string
* @param $x_coor float
* @param $y_coor float
* @param $mode (optional) int
*/
function cpdf_show_xy($pdf_document, $text, $x_coor, $y_coor, $mode);
/**
* Returns width of text in current font
* @return float
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $text string
*/
function cpdf_stringwidth($pdf_document, $text);
/**
* Draw line along path
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
*/
function cpdf_stroke($pdf_document);
/**
* Output text with parameters
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $text string
* @param $x_coor (optional) float
* @param $y_coor (optional) float
* @param $mode (optional) int
* @param $orientation (optional) float
* @param $alignmode (optional) int
*/
function cpdf_text($pdf_document, $text, $x_coor, $y_coor, $mode, $orientation, $alignmode);
/**
* Sets origin of coordinate system
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5 <= 5.0.4
* @param $pdf_document int
* @param $x_coor float
* @param $y_coor float
*/
function cpdf_translate($pdf_document, $x_coor, $y_coor);
/**
* Performs an obscure check with the given password
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $dictionary resource
* @param $password string
*/
function crack_check($dictionary, $password);
/**
* Closes an open CrackLib dictionary
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $dictionary resource
*/
function crack_closedict($dictionary);
/**
* Returns the message from the last obscure check
* @return string
* @version PHP 4 >= 4.0.5, PECL
*/
function crack_getlastmessage();
/**
* Opens a new CrackLib dictionary
* @return resource
* @version PHP 4 >= 4.0.5, PECL
* @param $dictionary string
*/
function crack_opendict($dictionary);
/**
* Calculates the crc32 polynomial of a string
* @return int
* @version PHP 4 >= 4.0.1, PHP 5
* @param $str string
*/
function crc32($str);
/**
* Create an anonymous (lambda-style) function
* @return string
* @version PHP 4 >= 4.0.1, PHP 5
* @param $args string
* @param $code string
*/
function create_function($args, $code);
/**
* One-way string encryption (hashing)
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $salt (optional) string
*/
function crypt($str, $salt);
/**
* Check for alphanumeric character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_alnum($text);
/**
* Check for alphabetic character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_alpha($text);
/**
* Check for control character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_cntrl($text);
/**
* Check for numeric character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_digit($text);
/**
* Check for any printable character(s) except space
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_graph($text);
/**
* Check for lowercase character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_lower($text);
/**
* Check for printable character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_print($text);
/**
* Check for any printable character which is not whitespace or an alphanumeric character
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_punct($text);
/**
* Check for whitespace character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_space($text);
/**
* Check for uppercase character(s)
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_upper($text);
/**
* Check for character(s) representing a hexadecimal digit
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $text string
*/
function ctype_xdigit($text);
/**
* Close a CURL session
* @return 
* @version PHP 4 >= 4.0.2, PHP 5
* @param $ch resource
*/
function curl_close($ch);
/**
* Copy a cURL handle along with all of its preferences
* @return resource
* @version PHP 5
* @param $ch resource
*/
function curl_copy_handle($ch);
/**
* Return the last error number
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $ch resource
*/
function curl_errno($ch);
/**
* Return a string containing the last error for the current session
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $ch resource
*/
function curl_error($ch);
/**
* Perform a CURL session
* @return mixed
* @version PHP 4 >= 4.0.2, PHP 5
* @param $ch resource
*/
function curl_exec($ch);
/**
* Get information regarding a specific transfer
* @return mixed
* @version PHP 4 >= 4.0.4, PHP 5
* @param $ch resource
* @param $opt (optional) int
*/
function curl_getinfo($ch, $opt);
/**
* Initialize a CURL session
* @return resource
* @version PHP 4 >= 4.0.2, PHP 5
* @param $url string
*/
function curl_init($url);
/**
* Add a normal cURL handle to a cURL multi handle
* @return int
* @version PHP 5
* @param $mh resource
* @param $ch resource
*/
function curl_multi_add_handle($mh, $ch);
/**
* Close a set of cURL handles
* @return 
* @version PHP 5
* @param $mh resource
*/
function curl_multi_close($mh);
/**
* Run the sub-connections of the current cURL handle
* @return int
* @version PHP 5
* @param $mh resource
* @param &$still_running int
*/
function curl_multi_exec($mh, &$still_running);
/**
* Return the content of a cURL handle if CURLOPT_RETURNTRANSFER is set
* @return string
* @version PHP 5
* @param $ch resource
*/
function curl_multi_getcontent($ch);
/**
* Get information about the current transfers
* @return array
* @version PHP 5
* @param $mh resource
*/
function curl_multi_info_read($mh);
/**
* Returns a new cURL multi handle
* @return resource
* @version PHP 5
*/
function curl_multi_init();
/**
* Remove a multi handle from a set of cURL handles
* @return int
* @version PHP 5
* @param $mh resource
* @param $ch resource
*/
function curl_multi_remove_handle($mh, $ch);
/**
* Get all the sockets associated with the cURL extension, which can then be "selected"
* @return int
* @version PHP 5
* @param $mh resource
* @param $timeout (optional) float
*/
function curl_multi_select($mh, $timeout);
/**
* Set an option for a CURL transfer
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $ch resource
* @param $option int
* @param $value mixed
*/
function curl_setopt($ch, $option, $value);
/**
* Set multiple options for a CURL transfer
* @return bool
* @version PHP 5 CVS only
* @param $ch resource
* @param $options array
*/
function curl_setopt_array($ch, $options);
/**
* Return the current CURL version
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $version int
*/
function curl_version($version);
/**
* Return the current element in an array
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function current(&$array);
/**
* base64 decode data for Cybercash
* @return string
* @version PHP 4 <= 4.2.3, PECL
* @param $inbuff string
*/
function cybercash_base64_decode($inbuff);
/**
* base64 encode data for Cybercash
* @return string
* @version PHP 4 <= 4.2.3, PECL
* @param $inbuff string
*/
function cybercash_base64_encode($inbuff);
/**
* Cybercash decrypt
* @return array
* @version PHP 4 <= 4.2.3, PECL
* @param $wmk string
* @param $sk string
* @param $inbuff string
*/
function cybercash_decr($wmk, $sk, $inbuff);
/**
* Cybercash encrypt
* @return array
* @version PHP 4 <= 4.2.3, PECL
* @param $wmk string
* @param $sk string
* @param $inbuff string
*/
function cybercash_encr($wmk, $sk, $inbuff);
/**
* Generate HTML form of request for payment
* @return string
* @version 4.0.5 - 4.2.3 only, PECL
* @param $url_cm string
* @param $version string
* @param $tpe string
* @param $price string
* @param $ref_command string
* @param $text_free string
* @param $url_return string
* @param $url_return_ok string
* @param $url_return_err string
* @param $language string
* @param $code_company string
* @param $text_button string
*/
function cybermut_creerformulairecm($url_cm, $version, $tpe, $price, $ref_command, $text_free, $url_return, $url_return_ok, $url_return_err, $language, $code_company, $text_button);
/**
* Generate the delivery's acknowledgement of the payment's confirmation
* @return string
* @version 4.0.5 - 4.2.3 only, PECL
* @param $sentence string
*/
function cybermut_creerreponsecm($sentence);
/**
* Make sure that there was no data diddling contained in the received message of confirmation
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $code_mac string
* @param $version string
* @param $tpe string
* @param $cdate string
* @param $price string
* @param $ref_command string
* @param $text_free string
* @param $code_return string
*/
function cybermut_testmac($code_mac, $version, $tpe, $cdate, $price, $ref_command, $text_free, $code_return);
/**
* Authenticate against a Cyrus IMAP server
* @return 
* @version PHP 4 >= 4.1.0, PECL
* @param $connection resource
* @param $mechlist (optional) string
* @param $service (optional) string
* @param $user (optional) string
* @param $minssf (optional) int
* @param $maxssf (optional) int
* @param $authname (optional) string
* @param $password (optional) string
*/
function cyrus_authenticate($connection, $mechlist, $service, $user, $minssf, $maxssf, $authname, $password);
/**
* Bind callbacks to a Cyrus IMAP connection
* @return bool
* @version PHP 4 >= 4.1.0, PECL
* @param $connection resource
* @param $callbacks array
*/
function cyrus_bind($connection, $callbacks);
/**
* Close connection to a Cyrus IMAP server
* @return bool
* @version PHP 4 >= 4.1.0, PECL
* @param $connection resource
*/
function cyrus_close($connection);
/**
* Connect to a Cyrus IMAP server
* @return resource
* @version PHP 4 >= 4.1.0, PECL
* @param $host string
* @param $port (optional) string
* @param $flags (optional) int
*/
function cyrus_connect($host, $port, $flags);
/**
* Send a query to a Cyrus IMAP server
* @return array
* @version PHP 4 >= 4.1.0, PECL
* @param $connection resource
* @param $query string
*/
function cyrus_query($connection, $query);
/**
* Unbind ...
* @return bool
* @version PHP 4 >= 4.1.0, PECL
* @param $connection resource
* @param $trigger_name string
*/
function cyrus_unbind($connection, $trigger_name);
/**
* Format a local time/date
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $timestamp (optional) int
*/
function date($format, $timestamp);
/**
* Gets the default timezone used by all date/time functions in a script
* @return string
* @version PHP 5 >= 5.1.0RC1
*/
function date_default_timezone_get();
/**
* Sets the default timezone used by all date/time functions in a script
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $timezone_identifier string
*/
function date_default_timezone_set($timezone_identifier);
/**
* Returns time of sunrise for a given day and location
* @return mixed
* @version PHP 5
* @param $timestamp int
* @param $format (optional) int
* @param $latitude (optional) float
* @param $longitude (optional) float
* @param $zenith (optional) float
* @param $gmt_offset (optional) float
*/
function date_sunrise($timestamp, $format, $latitude, $longitude, $zenith, $gmt_offset);
/**
* Returns time of sunset for a given day and location
* @return mixed
* @version PHP 5
* @param $timestamp int
* @param $format (optional) int
* @param $latitude (optional) float
* @param $longitude (optional) float
* @param $zenith (optional) float
* @param $gmt_offset (optional) float
*/
function date_sunset($timestamp, $format, $latitude, $longitude, $zenith, $gmt_offset);
/**
* Returns or sets the AUTOCOMMIT state for a database connection
* @return mixed
* @version PECL
* @param $connection resource
* @param $value (optional) bool
*/
function db2_autocommit($connection, $value);
/**
* Binds a PHP variable to an SQL statement parameter
* @return bool
* @version PECL
* @param $stmt resource
* @param $parameter-number int
* @param $variable-name string
* @param $parameter-type (optional) int
* @param $data-type (optional) int
* @param $precision (optional) int
* @param $scale (optional) int
*/
function db2_bind_param($stmt, $parameter-number, $variable-name, $parameter-type, $data-type, $precision, $scale);
/**
* Closes a database connection
* @return bool
* @version PECL
* @param $connection resource
*/
function db2_close($connection);
/**
* Returns a result set listing the columns and associated metadata for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier (optional) string
* @param $schema (optional) string
* @param $table-name (optional) string
* @param $column-name (optional) string
*/
function db2_columns($connection, $qualifier, $schema, $table-name, $column-name);
/**
* Returns a result set listing the columns and associated privileges for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier (optional) string
* @param $schema (optional) string
* @param $table-name (optional) string
* @param $column-name (optional) string
*/
function db2_column_privileges($connection, $qualifier, $schema, $table-name, $column-name);
/**
* Commits a transaction
* @return bool
* @version PECL
* @param $connection resource
*/
function db2_commit($connection);
/**
* Returns a connection to a database
* @return resource
* @version PECL
* @param $database string
* @param $username string
* @param $password string
* @param $options (optional) array
*/
function db2_connect($database, $username, $password, $options);
/**
* Returns a string containing the SQLSTATE returned by the last connection attempt
* @return string
* @version PECL
* @param $connection resource
*/
function db2_conn_error($connection);
/**
* Returns the last connection error message and SQLCODE value
* @return string
* @version PECL
* @param $connection resource
*/
function db2_conn_errormsg($connection);
/**
* Returns the cursor type used by a statement resource
* @return int
* @version PECL
* @param $stmt resource
*/
function db2_cursor_type($stmt);
/**
* Executes an SQL statement directly
* @return resource
* @version PECL
* @param $connection resource
* @param $statement string
* @param $options (optional) array
*/
function db2_exec($connection, $statement, $options);
/**
* Executes a prepared SQL statement
* @return bool
* @version PECL
* @param $stmt resource
* @param $parameters (optional) array
*/
function db2_execute($stmt, $parameters);
/**
* Returns an array, indexed by column position, representing a row in a result set
* @return array
* @version PECL
* @param $stmt resource
* @param $row_number (optional) int
*/
function db2_fetch_array($stmt, $row_number);
/**
* Returns an array, indexed by column name, representing a row in a result set
* @return array
* @version PECL
* @param $stmt resource
* @param $row_number (optional) int
*/
function db2_fetch_assoc($stmt, $row_number);
/**
* Returns an array, indexed by both column name and position, representing a row in a result set
* @return array
* @version PECL
* @param $stmt resource
* @param $row_number (optional) int
*/
function db2_fetch_both($stmt, $row_number);
/**
* Returns an object with properties representing columns in the fetched row
* @return object
* @version PECL
* @param $stmt resource
* @param $row_number (optional) int
*/
function db2_fetch_object($stmt, $row_number);
/**
* Sets the result set pointer to the next row or requested row
* @return bool
* @version PECL
* @param $stmt resource
* @param $row_number (optional) int
*/
function db2_fetch_row($stmt, $row_number);
/**
* Returns the maximum number of bytes required to display a column
* @return int
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_display_size($stmt, $column);
/**
* Returns the name of the column in the result set
* @return string
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_name($stmt, $column);
/**
* Returns the position of the named column in a result set
* @return int
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_num($stmt, $column);
/**
* Returns the precision of the indicated column in a result set
* @return int
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_precision($stmt, $column);
/**
* Returns the scale of the indicated column in a result set
* @return int
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_scale($stmt, $column);
/**
* Returns the data type of the indicated column in a result set
* @return string
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_type($stmt, $column);
/**
* Returns the width of the current value of the indicated column in a result set
* @return int
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_field_width($stmt, $column);
/**
* Returns a result set listing the foreign keys for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $table-name string
*/
function db2_foreign_keys($connection, $qualifier, $schema, $table-name);
/**
* Frees resources associated with a result set
* @return bool
* @version PECL
* @param $stmt resource
*/
function db2_free_result($stmt);
/**
* Frees resources associated with the indicated statement resource
* @return bool
* @version PECL
* @param $stmt resource
*/
function db2_free_stmt($stmt);
/**
* Requests the next result set from a stored procedure
* @return resource
* @version PECL
* @param $stmt resource
*/
function db2_next_result($stmt);
/**
* Returns the number of fields contained in a result set
* @return int
* @version PECL
* @param $stmt resource
*/
function db2_num_fields($stmt);
/**
* Returns the number of rows affected by an SQL statement
* @return int
* @version PECL
* @param $stmt resource
*/
function db2_num_rows($stmt);
/**
* Returns a persistent connection to a database
* @return resource
* @version PECL
* @param $database string
* @param $username string
* @param $password string
* @param $options (optional) array
*/
function db2_pconnect($database, $username, $password, $options);
/**
* Prepares an SQL statement to be executed
* @return resource
* @version PECL
* @param $connection resource
* @param $statement string
* @param $options (optional) array
*/
function db2_prepare($connection, $statement, $options);
/**
* Returns a result set listing primary keys for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $table-name string
*/
function db2_primary_keys($connection, $qualifier, $schema, $table-name);
/**
* Returns a result set listing the stored procedures registered in a database
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $procedure string
*/
function db2_procedures($connection, $qualifier, $schema, $procedure);
/**
* Returns a result set listing stored procedure parameters
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $procedure string
* @param $parameter string
*/
function db2_procedure_columns($connection, $qualifier, $schema, $procedure, $parameter);
/**
* Returns a single column from a row in the result set
* @return mixed
* @version PECL
* @param $stmt resource
* @param $column mixed
*/
function db2_result($stmt, $column);
/**
* Rolls back a transaction
* @return bool
* @version PECL
* @param $connection resource
*/
function db2_rollback($connection);
/**
* Returns a result set listing the unique row identifier columns for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $table_name string
* @param $scope int
*/
function db2_special_columns($connection, $qualifier, $schema, $table_name, $scope);
/**
* Returns a result set listing the index and statistics for a table
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier string
* @param $schema string
* @param $table-name string
* @param $unique bool
*/
function db2_statistics($connection, $qualifier, $schema, $table-name, $unique);
/**
* Returns a string containing the SQLSTATE returned by an SQL statement
* @return string
* @version PECL
* @param $stmt resource
*/
function db2_stmt_error($stmt);
/**
* Returns a string containing the last SQL statement error message
* @return string
* @version PECL
* @param $stmt resource
*/
function db2_stmt_errormsg($stmt);
/**
* Returns a result set listing the tables and associated metadata in a database
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier (optional) string
* @param $schema (optional) string
* @param $table-name (optional) string
* @param $table-type (optional) string
*/
function db2_tables($connection, $qualifier, $schema, $table-name, $table-type);
/**
* Returns a result set listing the tables and associated privileges in a database
* @return resource
* @version PECL
* @param $connection resource
* @param $qualifier (optional) string
* @param $schema (optional) string
* @param $table_name (optional) string
*/
function db2_table_privileges($connection, $qualifier, $schema, $table_name);
/**
* Adds a record to a database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
* @param $record array
*/
function dbase_add_record($dbase_identifier, $record);
/**
* Closes a database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
*/
function dbase_close($dbase_identifier);
/**
* Creates a database
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $fields array
*/
function dbase_create($filename, $fields);
/**
* Deletes a record from a database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
* @param $record_number int
*/
function dbase_delete_record($dbase_identifier, $record_number);
/**
* Gets the header info of a database
* @return array
* @version PHP 5
* @param $dbase_identifier int
*/
function dbase_get_header_info($dbase_identifier);
/**
* Gets a record from a database as an indexed array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
* @param $record_number int
*/
function dbase_get_record($dbase_identifier, $record_number);
/**
* Gets a record from a database as an associative array
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $dbase_identifier int
* @param $record_number int
*/
function dbase_get_record_with_names($dbase_identifier, $record_number);
/**
* Gets the number of fields of a database
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
*/
function dbase_numfields($dbase_identifier);
/**
* Gets the number of records in a database
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
*/
function dbase_numrecords($dbase_identifier);
/**
* Opens a database
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $mode int
*/
function dbase_open($filename, $mode);
/**
* Packs a database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dbase_identifier int
*/
function dbase_pack($dbase_identifier);
/**
* Replaces a record in a database
* @return bool
* @version PHP 3 >= 3.0.11, PHP 4, PHP 5
* @param $dbase_identifier int
* @param $record array
* @param $record_number int
*/
function dbase_replace_record($dbase_identifier, $record, $record_number);
/**
* Close a DBA database
* @return 
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
*/
function dba_close($handle);
/**
* Delete DBA entry specified by key
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $key string
* @param $handle resource
*/
function dba_delete($key, $handle);
/**
* Check whether key exists
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $key string
* @param $handle resource
*/
function dba_exists($key, $handle);
/**
* Fetch data specified by key
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $key string
* @param $handle resource
*/
function dba_fetch($key, $handle);
/**
* Fetch first key
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
*/
function dba_firstkey($handle);
/**
* List all the handlers available
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $full_info bool
*/
function dba_handlers($full_info);
/**
* Insert entry
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $key string
* @param $value string
* @param $handle resource
*/
function dba_insert($key, $value, $handle);
/**
* Splits a key in string representation into array representation
* @return mixed
* @version PHP 5
* @param $key mixed
*/
function dba_key_split($key);
/**
* List all open database files
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function dba_list();
/**
* Fetch next key
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
*/
function dba_nextkey($handle);
/**
* Open database
* @return resource
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $path string
* @param $mode string
* @param $handler (optional) string
* @param $params1 (optional) mixed
*/
function dba_open($path, $mode, $handler, $params1);
/**
* Optimize database
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
*/
function dba_optimize($handle);
/**
* Open database persistently
* @return resource
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $path string
* @param $mode string
* @param $handler (optional) string
* @param $params1 (optional) mixed
*/
function dba_popen($path, $mode, $handler, $params1);
/**
* Replace or insert entry
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $key string
* @param $value string
* @param $handle resource
*/
function dba_replace($key, $value, $handle);
/**
* Synchronize database
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
*/
function dba_sync($handle);
/**
* Describes the DBM-compatible library being used
* @return string
* @version PHP 3, PHP 4, PECL
*/
function dblist();
/**
* Closes a dbm database
* @return bool
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
*/
function dbmclose($dbm_identifier);
/**
* Deletes the value for a key from a DBM database
* @return bool
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
*/
function dbmdelete($dbm_identifier, $key);
/**
* Tells if a value exists for a key in a DBM database
* @return bool
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
*/
function dbmexists($dbm_identifier, $key);
/**
* Fetches a value for a key from a DBM database
* @return string
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
*/
function dbmfetch($dbm_identifier, $key);
/**
* Retrieves the first key from a DBM database
* @return string
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
*/
function dbmfirstkey($dbm_identifier);
/**
* Inserts a value for a key in a DBM database
* @return int
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
* @param $value string
*/
function dbminsert($dbm_identifier, $key, $value);
/**
* Retrieves the next key from a DBM database
* @return string
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
*/
function dbmnextkey($dbm_identifier, $key);
/**
* Opens a DBM database
* @return resource
* @version PHP 3, PHP 4, PECL
* @param $filename string
* @param $flags string
*/
function dbmopen($filename, $flags);
/**
* Replaces the value for a key in a DBM database
* @return int
* @version PHP 3, PHP 4, PECL
* @param $dbm_identifier resource
* @param $key string
* @param $value string
*/
function dbmreplace($dbm_identifier, $key, $value);
/**
* Add a tuple to a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $tuple array
*/
function dbplus_add($relation, $tuple);
/**
* Perform AQL query
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $query string
* @param $server (optional) string
* @param $dbpath (optional) string
*/
function dbplus_aql($query, $server, $dbpath);
/**
* Get/Set database virtual current directory
* @return string
* @version 4.1.0 - 4.2.3 only, PECL
* @param $newdir string
*/
function dbplus_chdir($newdir);
/**
* Close a relation
* @return mixed
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_close($relation);
/**
* Get current tuple from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param &$tuple array
*/
function dbplus_curr($relation, &$tuple);
/**
* Get error string for given errorcode or last error
* @return string
* @version 4.1.0 - 4.2.3 only, PECL
* @param $errno int
*/
function dbplus_errcode($errno);
/**
* Get error code for last operation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
*/
function dbplus_errno();
/**
* Set a constraint on a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $constraints array
* @param $tuple mixed
*/
function dbplus_find($relation, $constraints, $tuple);
/**
* Get first tuple from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param &$tuple array
*/
function dbplus_first($relation, &$tuple);
/**
* Flush all changes made on a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_flush($relation);
/**
* Free all locks held by this client
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
*/
function dbplus_freealllocks();
/**
* Release write lock on tuple
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $tname string
*/
function dbplus_freelock($relation, $tname);
/**
* Free all tuple locks on given relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_freerlocks($relation);
/**
* Get a write lock on a tuple
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $tname string
*/
function dbplus_getlock($relation, $tname);
/**
* Get an id number unique to a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $uniqueid int
*/
function dbplus_getunique($relation, $uniqueid);
/**
* Get information about a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $key string
* @param &$result array
*/
function dbplus_info($relation, $key, &$result);
/**
* Get last tuple from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param &$tuple array
*/
function dbplus_last($relation, &$tuple);
/**
* Get next tuple from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param &$tuple array
*/
function dbplus_next($relation, &$tuple);
/**
* Open relation file
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $name string
*/
function dbplus_open($name);
/**
* Get previous tuple from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param &$tuple array
*/
function dbplus_prev($relation, &$tuple);
/**
* Change relation permissions
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $mask int
* @param $user string
* @param $group string
*/
function dbplus_rchperm($relation, $mask, $user, $group);
/**
* Creates a new DB++ relation
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $name string
* @param $domlist mixed
* @param $overwrite (optional) bool
*/
function dbplus_rcreate($name, $domlist, $overwrite);
/**
* Creates an exact but empty copy of a relation including indices
* @return mixed
* @version 4.1.0 - 4.2.3 only, PECL
* @param $name string
* @param $relation resource
* @param $overwrite (optional) bool
*/
function dbplus_rcrtexact($name, $relation, $overwrite);
/**
* Creates an empty copy of a relation with default indices
* @return mixed
* @version 4.1.0 - 4.2.3 only, PECL
* @param $name string
* @param $relation resource
* @param $overwrite (optional) int
*/
function dbplus_rcrtlike($name, $relation, $overwrite);
/**
* Resolve host information for relation
* @return array
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation_name string
*/
function dbplus_resolve($relation_name);
/**
* Restore position
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $tuple array
*/
function dbplus_restorepos($relation, $tuple);
/**
* Specify new primary key for a relation
* @return mixed
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $domlist mixed
*/
function dbplus_rkeys($relation, $domlist);
/**
* Open relation file local
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $name string
*/
function dbplus_ropen($name);
/**
* Perform local (raw) AQL query
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $query string
* @param $dbpath (optional) string
*/
function dbplus_rquery($query, $dbpath);
/**
* Rename a relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $name string
*/
function dbplus_rrename($relation, $name);
/**
* Create a new secondary index for a relation
* @return mixed
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $domlist mixed
* @param $type int
*/
function dbplus_rsecindex($relation, $domlist, $type);
/**
* Remove relation from filesystem
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_runlink($relation);
/**
* Remove all tuples from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_rzap($relation);
/**
* Save position
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_savepos($relation);
/**
* Set index
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $idx_name string
*/
function dbplus_setindex($relation, $idx_name);
/**
* Set index by number
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $idx_number int
*/
function dbplus_setindexbynumber($relation, $idx_number);
/**
* Perform SQL query
* @return resource
* @version 4.1.0 - 4.2.3 only, PECL
* @param $query string
* @param $server (optional) string
* @param $dbpath (optional) string
*/
function dbplus_sql($query, $server, $dbpath);
/**
* Execute TCL code on server side
* @return string
* @version 4.1.0 - 4.2.3 only, PECL
* @param $sid int
* @param $script string
*/
function dbplus_tcl($sid, $script);
/**
* Remove tuple and return new current tuple
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $tuple array
* @param &$current (optional) array
*/
function dbplus_tremove($relation, $tuple, &$current);
/**
* Undo
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_undo($relation);
/**
* Prepare undo
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_undoprepare($relation);
/**
* Give up write lock on relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_unlockrel($relation);
/**
* Remove a constraint from relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_unselect($relation);
/**
* Update specified tuple in relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
* @param $old array
* @param $new array
*/
function dbplus_update($relation, $old, $new);
/**
* Request exclusive lock on relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_xlockrel($relation);
/**
* Free exclusive lock on relation
* @return int
* @version 4.1.0 - 4.2.3 only, PECL
* @param $relation resource
*/
function dbplus_xunlockrel($relation);
/**
* Close an open connection/database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $link_identifier object
*/
function dbx_close($link_identifier);
/**
* Compare two rows for sorting purposes
* @return int
* @version PHP 4 >= 4.1.0, PHP 5 <= 5.0.4
* @param $row_a array
* @param $row_b array
* @param $column_key string
* @param $flags (optional) int
*/
function dbx_compare($row_a, $row_b, $column_key, $flags);
/**
* Open a connection/database
* @return object
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $module mixed
* @param $host string
* @param $database string
* @param $username string
* @param $password string
* @param $persistent (optional) int
*/
function dbx_connect($module, $host, $database, $username, $password, $persistent);
/**
* Report the error message of the latest function call in the module (not just in the connection)
* @return string
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $link_identifier object
*/
function dbx_error($link_identifier);
/**
* Escape a string so it can safely be used in an sql-statement
* @return string
* @version PHP 4 >= 4.3.0, PHP 5 <= 5.0.4
* @param $link_identifier object
* @param $text string
*/
function dbx_escape_string($link_identifier, $text);
/**
* Fetches rows from a query-result that had the DBX_RESULT_UNBUFFERED flag set
* @return mixed
* @version PHP 5 <= 5.0.4
* @param $result_identifier object
*/
function dbx_fetch_row($result_identifier);
/**
* Send a query and fetch all results (if any)
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $link_identifier object
* @param $sql_statement string
* @param $flags (optional) int
*/
function dbx_query($link_identifier, $sql_statement, $flags);
/**
* Sort a result from a dbx_query by a custom sort function
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $result object
* @param $user_compare_function string
*/
function dbx_sort($result, $user_compare_function);
/**
* Overrides the domain for a single lookup
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $domain string
* @param $message string
* @param $category int
*/
function dcgettext($domain, $message, $category);
/**
* Plural version of dcgettext
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $domain string
* @param $msgid1 string
* @param $msgid2 string
* @param $n int
* @param $category int
*/
function dcngettext($domain, $msgid1, $msgid2, $n, $category);
/**
* Removes the aggregated methods and properties from an object
* @return 
* @version PHP 4 >= 4.2.0
* @param $object object
* @param $class_name (optional) string
*/
function deaggregate($object, $class_name);
/**
* Disable internal PHP debugger (PHP 3)
* @return int
* @version PHP 3
*/
function debugger_off();
/**
* Enable internal PHP debugger (PHP 3)
* @return int
* @version PHP 3
* @param $address string
*/
function debugger_on($address);
/**
* Generates a backtrace
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function debug_backtrace();
/**
* Prints a backtrace
* @return 
* @version PHP 5
*/
function debug_print_backtrace();
/**
* Dumps a string representation of an internal zend value to output
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $variable mixed
*/
function debug_zval_dump($variable);
/**
* Decimal to binary
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $number int
*/
function decbin($number);
/**
* Decimal to hexadecimal
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $number int
*/
function dechex($number);
/**
* Decimal to octal
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $number int
*/
function decoct($number);
/**
* Defines a named constant
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $name string
* @param $value mixed
* @param $case_insensitive (optional) bool
*/
function define($name, $value, $case_insensitive);
/**
* Checks whether a given named constant exists
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $name string
*/
function defined($name);
/**
* Initializes all syslog related constants
* @return 
* @version PHP 3, PHP 4, PHP 5
*/
function define_syslog_variables();
/**
* Converts the number in degrees to the radian equivalent
* @return float
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $number float
*/
function deg2rad($number);
/**
* Override the current domain
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $domain string
* @param $message string
*/
function dgettext($domain, $message);
/**
* Equivalent to exit()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function die();
/**
* Closes the file descriptor given by fd
* @return 
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
*/
function dio_close($fd);
/**
* Performs a c library fcntl on fd
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $cmd int
* @param $args (optional) mixed
*/
function dio_fcntl($fd, $cmd, $args);
/**
* Opens a new filename with specified permissions of flags and creation permissions of mode
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $filename string
* @param $flags int
* @param $mode (optional) int
*/
function dio_open($filename, $flags, $mode);
/**
* Reads bytes from a file descriptor
* @return string
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $len (optional) int
*/
function dio_read($fd, $len);
/**
* Seeks to pos on fd from whence
* @return int
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $pos int
* @param $whence (optional) int
*/
function dio_seek($fd, $pos, $whence);
/**
* Gets stat information about the file descriptor fd
* @return array
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
*/
function dio_stat($fd);
/**
* Sets terminal attributes and baud rate for a serial port
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $options array
*/
function dio_tcsetattr($fd, $options);
/**
* Truncates file descriptor fd to offset bytes
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $offset int
*/
function dio_truncate($fd, $offset);
/**
* Writes data to fd with optional truncation at length
* @return int
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $fd resource
* @param $data string
* @param $len (optional) int
*/
function dio_write($fd, $data, $len);
/**
* Returns directory name component of path
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $path string
*/
function dirname($path);
/**
* Alias of disk_free_space()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function diskfreespace();
/**
* Returns available space in directory
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $directory string
*/
function disk_free_space($directory);
/**
* Returns the total size of a directory
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $directory string
*/
function disk_total_space($directory);
/**
* Loads a PHP extension at runtime
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $library string
*/
function dl($library);
/**
* Plural version of dgettext
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $domain string
* @param $msgid1 string
* @param $msgid2 string
* @param $n int
*/
function dngettext($domain, $msgid1, $msgid2, $n);
/**
* Synonym for checkdnsrr()
* @return bool
* @version PHP 5
* @param $host string
* @param $type (optional) string
*/
function dns_check_record($host, $type);
/**
* Synonym for getmxrr()
* @return bool
* @version PHP 5
* @param $hostname string
* @param &$mxhosts array
* @param &$weight (optional) array
*/
function dns_get_mx($hostname, &$mxhosts, &$weight);
/**
* Fetch DNS Resource Records associated with a hostname
* @return array
* @version PHP 5
* @param $hostname string
* @param $type (optional) int
* @param &$authns (optional) array
* @param &$addtl (optional) array
*/
function dns_get_record($hostname, $type, &$authns, &$addtl);
/**
* Creates new empty XML document
* @return DomDocument
* @version PHP 4 >= 4.2.1, PECL
* @param $version string
*/
function domxml_new_doc($version);
/**
* Creates a DOM object from an XML file
* @return DomDocument
* @version PHP 4 >= 4.2.1, PECL
* @param $filename string
* @param $mode (optional) int
* @param &$error (optional) array
*/
function domxml_open_file($filename, $mode, &$error);
/**
* Creates a DOM object of an XML document
* @return DomDocument
* @version PHP 4 >= 4.2.1, PECL
* @param $str string
* @param $mode (optional) int
* @param &$error (optional) array
*/
function domxml_open_mem($str, $mode, &$error);
/**
* Gets the XML library version
* @return string
* @version PHP 4 >= 4.1.0, PECL
*/
function domxml_version();
/**
* Creates a tree of PHP objects from an XML document
* @return DomDocument
* @version PHP 4 >= 4.2.1, PECL
* @param $str string
*/
function domxml_xmltree($str);
/**
* Creates a DomXsltStylesheet object from an XSL document in a string
* @return DomXsltStylesheet
* @version PHP 4 >= 4.2.0, PECL
* @param $xsl_buf string
*/
function domxml_xslt_stylesheet($xsl_buf);
/**
* Creates a DomXsltStylesheet Object from a DomDocument Object
* @return DomXsltStylesheet
* @version PHP 4 >= 4.2.0, PECL
* @param $xsl_doc DomDocument
*/
function domxml_xslt_stylesheet_doc($xsl_doc);
/**
* Creates a DomXsltStylesheet Object from an XSL document in a file
* @return DomXsltStylesheet
* @version PHP 4 >= 4.2.0, PECL
* @param $xsl_file string
*/
function domxml_xslt_stylesheet_file($xsl_file);
/**
* Gets the XSLT library version
* @return int
* @version PHP 4 >= 4.2.0, PECL
*/
function domxml_xslt_version();
/**
* Gets a DOMElement object from a SimpleXMLElement object
* @return DOMElement
* @version PHP 5
* @param $node SimpleXMLElement
*/
function dom_import_simplexml($node);
/**
* Alias of floatval()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function doubleval();
/**
* Return the current key and value pair from an array and advance the array cursor
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function each(&$array);
/**
* Get Unix timestamp for midnight on Easter of a given year
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $year int
*/
function easter_date($year);
/**
* Get number of days after March 21 on which Easter falls for a given year
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $year int
* @param $method (optional) int
*/
function easter_days($year, $method);
/**
* Translate string from EBCDIC to ASCII
* @return int
* @version PHP 3 >= 3.0.17
* @param $ebcdic_str string
*/
function ebcdic2ascii($ebcdic_str);
/**
* Output one or more strings
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $arg1 string
* @param $params1 (optional) string
*/
function echo($arg1, $params1);
/**
* Determine whether a variable is empty
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function empty($var);
/**
* Set the internal pointer of an array to its last element
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function end(&$array);
/**
* Regular expression match
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $pattern string
* @param $string string
* @param &$regs (optional) array
*/
function ereg($pattern, $string, &$regs);
/**
* Case insensitive regular expression match
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $pattern string
* @param $string string
* @param &$regs (optional) array
*/
function eregi($pattern, $string, &$regs);
/**
* Replace regular expression case insensitive
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $pattern string
* @param $replacement string
* @param $string string
*/
function eregi_replace($pattern, $replacement, $string);
/**
* Replace regular expression
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $pattern string
* @param $replacement string
* @param $string string
*/
function ereg_replace($pattern, $replacement, $string);
/**
* Send an error message somewhere
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $message string
* @param $message_type (optional) int
* @param $destination (optional) string
* @param $extra_headers (optional) string
*/
function error_log($message, $message_type, $destination, $extra_headers);
/**
* Sets which PHP errors are reported
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $level int
*/
function error_reporting($level);
/**
* Escape a string to be used as a shell argument
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $arg string
*/
function escapeshellarg($arg);
/**
* Escape shell metacharacters
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $command string
*/
function escapeshellcmd($command);
/**
* Evaluate a string as PHP code
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $code_str string
*/
function eval($code_str);
/**
* Execute an external program
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $command string
* @param &$output (optional) array
* @param &$return_var (optional) int
*/
function exec($command, &$output, &$return_var);
/**
* Determine the type of an image
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $filename string
*/
function exif_imagetype($filename);
/**
* Reads the EXIF headers from JPEG or TIFF
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
* @param $sections (optional) string
* @param $arrays (optional) bool
* @param $thumbnail (optional) bool
*/
function exif_read_data($filename, $sections, $arrays, $thumbnail);
/**
* Get the header name for an index
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $index string
*/
function exif_tagname($index);
/**
* Retrieve the embedded thumbnail of a TIFF or JPEG image
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
* @param &$width (optional) int
* @param &$height (optional) int
* @param &$imagetype (optional) int
*/
function exif_thumbnail($filename, &$width, &$height, &$imagetype);
/**
* Output a message and terminate the current script
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $status string
*/
function exit($status);
/**
* Calculates the exponent of e
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function exp($arg);
/**
* Split a string by string
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $separator string
* @param $string string
* @param $limit (optional) int
*/
function explode($separator, $string, $limit);
/**
* Returns exp(number) - 1, computed in a way that is accurate even when the value of number is close to zero
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $number float
*/
function expm1($number);
/**
* Find out whether an extension is loaded
* @return bool
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
* @param $name string
*/
function extension_loaded($name);
/**
* Import variables into the current symbol table from an array
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $var_array array
* @param $extract_type (optional) int
* @param $prefix (optional) string
*/
function extract($var_array, $extract_type, $prefix);
/**
* Calculate the hash value needed by EZMLM
* @return int
* @version PHP 3 >= 3.0.17, PHP 4 >= 4.0.2, PHP 5
* @param $addr string
*/
function ezmlm_hash($addr);
/**
* Terminate monitoring
* @return bool
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $fam_monitor resource
*/
function fam_cancel_monitor($fam, $fam_monitor);
/**
* Close FAM connection
* @return 
* @version PHP 5 <= 5.0.4
* @param $fam resource
*/
function fam_close($fam);
/**
* Monitor a collection of files in a directory for changes
* @return resource
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $dirname string
* @param $depth int
* @param $mask string
*/
function fam_monitor_collection($fam, $dirname, $depth, $mask);
/**
* Monitor a directory for changes
* @return resource
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $dirname string
*/
function fam_monitor_directory($fam, $dirname);
/**
* Monitor a regular file for changes
* @return resource
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $filename string
*/
function fam_monitor_file($fam, $filename);
/**
* Get next pending FAM event
* @return array
* @version PHP 5 <= 5.0.4
* @param $fam resource
*/
function fam_next_event($fam);
/**
* Open connection to FAM daemon
* @return resource
* @version PHP 5 <= 5.0.4
* @param $appname string
*/
function fam_open($appname);
/**
* Check for pending FAM events
* @return int
* @version PHP 5 <= 5.0.4
* @param $fam resource
*/
function fam_pending($fam);
/**
* Resume suspended monitoring
* @return bool
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $fam_monitor resource
*/
function fam_resume_monitor($fam, $fam_monitor);
/**
* Temporarily suspend monitoring
* @return bool
* @version PHP 5 <= 5.0.4
* @param $fam resource
* @param $fam_monitor resource
*/
function fam_suspend_monitor($fam, $fam_monitor);
/**
* Get number of affected rows in previous FrontBase operation
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_affected_rows($link_identifier);
/**
* Enable or disable autocommit
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $OnOff (optional) bool
*/
function fbsql_autocommit($link_identifier, $OnOff);
/**
* Get the size of a BLOB
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $blob_handle string
* @param $link_identifier (optional) resource
*/
function fbsql_blob_size($blob_handle, $link_identifier);
/**
* Get the size of a CLOB
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $clob_handle string
* @param $link_identifier (optional) resource
*/
function fbsql_clob_size($clob_handle, $link_identifier);
/**
* Close FrontBase connection
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_close($link_identifier);
/**
* Commits a transaction to the database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_commit($link_identifier);
/**
* Open a connection to a FrontBase Server
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $hostname string
* @param $username (optional) string
* @param $password (optional) string
*/
function fbsql_connect($hostname, $username, $password);
/**
* Create a BLOB
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $blob_data string
* @param $link_identifier (optional) resource
*/
function fbsql_create_blob($blob_data, $link_identifier);
/**
* Create a CLOB
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $clob_data string
* @param $link_identifier (optional) resource
*/
function fbsql_create_clob($clob_data, $link_identifier);
/**
* Create a FrontBase database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
* @param $database_options (optional) string
*/
function fbsql_create_db($database_name, $link_identifier, $database_options);
/**
* Get or set the database name used with a connection
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $database (optional) string
*/
function fbsql_database($link_identifier, $database);
/**
* Sets or retrieves the password for a FrontBase database
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $database_password (optional) string
*/
function fbsql_database_password($link_identifier, $database_password);
/**
* Move internal result pointer
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result_identifier resource
* @param $row_number int
*/
function fbsql_data_seek($result_identifier, $row_number);
/**
* Send a FrontBase query
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database string
* @param $query string
* @param $link_identifier (optional) resource
*/
function fbsql_db_query($database, $query, $link_identifier);
/**
* Get the status for a given database
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function fbsql_db_status($database_name, $link_identifier);
/**
* Drop (delete) a FrontBase database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function fbsql_drop_db($database_name, $link_identifier);
/**
* Returns the numerical value of the error message from previous FrontBase operation
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_errno($link_identifier);
/**
* Returns the text of the error message from previous FrontBase operation
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_error($link_identifier);
/**
* Fetch a result row as an associative array, a numeric array, or both
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $result_type (optional) int
*/
function fbsql_fetch_array($result, $result_type);
/**
* Fetch a result row as an associative array
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_fetch_assoc($result);
/**
* Get column information from a result and return as an object
* @return object
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_fetch_field($result, $field_offset);
/**
* Get the length of each output in a result
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_fetch_lengths($result);
/**
* Fetch a result row as an object
* @return object
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $result_type (optional) int
*/
function fbsql_fetch_object($result, $result_type);
/**
* Get a result row as an enumerated array
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_fetch_row($result);
/**
* Get the flags associated with the specified field in a result
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_field_flags($result, $field_offset);
/**
* Returns the length of the specified field
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_field_len($result, $field_offset);
/**
* Get the name of the specified field in a result
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_index (optional) int
*/
function fbsql_field_name($result, $field_index);
/**
* Set result pointer to a specified field offset
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_field_seek($result, $field_offset);
/**
* Get name of the table the specified field is in
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_field_table($result, $field_offset);
/**
* Get the type of the specified field in a result
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function fbsql_field_type($result, $field_offset);
/**
* Free result memory
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_free_result($result);
/**
* No description given yet
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $link_identifier resource
*/
function fbsql_get_autostart_info($link_identifier);
/**
* Get or set the host name used with a connection
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $host_name (optional) string
*/
function fbsql_hostname($link_identifier, $host_name);
/**
* Get the id generated from the previous INSERT operation
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_insert_id($link_identifier);
/**
* List databases available on a FrontBase server
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_list_dbs($link_identifier);
/**
* List FrontBase result fields
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $table_name string
* @param $link_identifier (optional) resource
*/
function fbsql_list_fields($database_name, $table_name, $link_identifier);
/**
* List tables in a FrontBase database
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database string
* @param $link_identifier (optional) resource
*/
function fbsql_list_tables($database, $link_identifier);
/**
* Move the internal result pointer to the next result
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result_id resource
*/
function fbsql_next_result($result_id);
/**
* Get number of fields in result
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_num_fields($result);
/**
* Get number of rows in result
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
*/
function fbsql_num_rows($result);
/**
* Get or set the user password used with a connection
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $password (optional) string
*/
function fbsql_password($link_identifier, $password);
/**
* Open a persistent connection to a FrontBase Server
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $hostname string
* @param $username (optional) string
* @param $password (optional) string
*/
function fbsql_pconnect($hostname, $username, $password);
/**
* Send a FrontBase query
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
* @param $batch_size (optional) int
*/
function fbsql_query($query, $link_identifier, $batch_size);
/**
* Read a BLOB from the database
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $blob_handle string
* @param $link_identifier (optional) resource
*/
function fbsql_read_blob($blob_handle, $link_identifier);
/**
* Read a CLOB from the database
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $clob_handle string
* @param $link_identifier (optional) resource
*/
function fbsql_read_clob($clob_handle, $link_identifier);
/**
* Get result data
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $result resource
* @param $row (optional) int
* @param $field (optional) mixed
*/
function fbsql_result($result, $row, $field);
/**
* Rollback a transaction to the database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
*/
function fbsql_rollback($link_identifier);
/**
* Select a FrontBase database
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function fbsql_select_db($database_name, $link_identifier);
/**
* Set the LOB retrieve mode for a FrontBase result set
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $database_name string
*/
function fbsql_set_lob_mode($result, $database_name);
/**
* Change the password for a given user
* @return bool
* @version PHP 5
* @param $link_identifier resource
* @param $user string
* @param $password string
* @param $old_password string
*/
function fbsql_set_password($link_identifier, $user, $password, $old_password);
/**
* Set the transaction locking and isolation
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $link_identifier resource
* @param $Locking int
* @param $Isolation int
*/
function fbsql_set_transaction($link_identifier, $Locking, $Isolation);
/**
* Start a database on local or remote server
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
* @param $database_options (optional) string
*/
function fbsql_start_db($database_name, $link_identifier, $database_options);
/**
* Stop a database on local or remote server
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function fbsql_stop_db($database_name, $link_identifier);
/**
* Get table name of field
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $i int
*/
function fbsql_tablename($result, $i);
/**
* Get or set the host user used with a connection
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $link_identifier resource
* @param $username (optional) string
*/
function fbsql_username($link_identifier, $username);
/**
* Enable or disable FrontBase warnings
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $OnOff bool
*/
function fbsql_warnings($OnOff);
/**
* Closes an open file pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function fclose($handle);
/**
* Adds javascript code to the FDF document
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdoc resource
* @param $script_name string
* @param $script_code string
*/
function fdf_add_doc_javascript($fdfdoc, $script_name, $script_code);
/**
* Adds a template into the FDF document
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $fdfdoc resource
* @param $newpage int
* @param $filename string
* @param $template string
* @param $rename int
*/
function fdf_add_template($fdfdoc, $newpage, $filename, $template, $rename);
/**
* Close an FDF document
* @return 
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
*/
function fdf_close($fdf_document);
/**
* Create a new FDF document
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
*/
function fdf_create();
/**
* Call a user defined function for each document value
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdoc resource
* @param $function callback
* @param $userdata (optional) mixed
*/
function fdf_enum_values($fdfdoc, $function, $userdata);
/**
* Return error code for last fdf operation
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
*/
function fdf_errno();
/**
* Return error description for fdf error code
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $error_code int
*/
function fdf_error($error_code);
/**
* Get the appearance of a field
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
* @param $field string
* @param $face int
* @param $filename string
*/
function fdf_get_ap($fdf_document, $field, $face, $filename);
/**
* Extracts uploaded file embedded in the FDF
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $savepath string
*/
function fdf_get_attachment($fdf_document, $fieldname, $savepath);
/**
* Get the value of the /Encoding key
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
*/
function fdf_get_encoding($fdf_document);
/**
* Get the value of the /F key
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
*/
function fdf_get_file($fdf_document);
/**
* Gets the flags of a field
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdoc resource
* @param $fieldname string
* @param $whichflags int
*/
function fdf_get_flags($fdfdoc, $fieldname, $whichflags);
/**
* Gets a value from the opt array of a field
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdof resource
* @param $fieldname string
* @param $element (optional) int
*/
function fdf_get_opt($fdfdof, $fieldname, $element);
/**
* Get the value of the /STATUS key
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
*/
function fdf_get_status($fdf_document);
/**
* Get the value of a field
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $which (optional) int
*/
function fdf_get_value($fdf_document, $fieldname, $which);
/**
* Gets version number for FDF API or file
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
*/
function fdf_get_version($fdf_document);
/**
* Sets FDF-specific output headers
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
*/
function fdf_header();
/**
* Get the next field name
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $fieldname (optional) string
*/
function fdf_next_field_name($fdf_document, $fieldname);
/**
* Open a FDF document
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $filename string
*/
function fdf_open($filename);
/**
* Read a FDF document from a string
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_data string
*/
function fdf_open_string($fdf_data);
/**
* Sets target frame for form
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdoc resource
* @param $fieldname string
* @param $item int
*/
function fdf_remove_item($fdfdoc, $fieldname, $item);
/**
* Save a FDF document
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $filename (optional) string
*/
function fdf_save($fdf_document, $filename);
/**
* Returns the FDF document as a string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
*/
function fdf_save_string($fdf_document);
/**
* Set the appearance of a field
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $field_name string
* @param $face int
* @param $filename string
* @param $page_number int
*/
function fdf_set_ap($fdf_document, $field_name, $face, $filename, $page_number);
/**
* Sets FDF character encoding
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $fdf_document resource
* @param $encoding string
*/
function fdf_set_encoding($fdf_document, $encoding);
/**
* Set PDF document to display FDF data in
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $url string
* @param $target_frame (optional) string
*/
function fdf_set_file($fdf_document, $url, $target_frame);
/**
* Sets a flag of a field
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $whichFlags int
* @param $newFlags int
*/
function fdf_set_flags($fdf_document, $fieldname, $whichFlags, $newFlags);
/**
* Sets an javascript action of a field
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $trigger int
* @param $script string
*/
function fdf_set_javascript_action($fdf_document, $fieldname, $trigger, $script);
/**
* Adds javascript code to be executed when Acrobat opens the FDF
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdfdoc resource
* @param $script string
* @param $before_data_import bool
*/
function fdf_set_on_import_javascript($fdfdoc, $script, $before_data_import);
/**
* Sets an option of a field
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $element int
* @param $str1 string
* @param $str2 string
*/
function fdf_set_opt($fdf_document, $fieldname, $element, $str1, $str2);
/**
* Set the value of the /STATUS key
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $status string
*/
function fdf_set_status($fdf_document, $status);
/**
* Sets a submit form action of a field
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $trigger int
* @param $script string
* @param $flags int
*/
function fdf_set_submit_form_action($fdf_document, $fieldname, $trigger, $script, $flags);
/**
* Set target frame for form display
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
* @param $frame_name string
*/
function fdf_set_target_frame($fdf_document, $frame_name);
/**
* Set the value of a field
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $fdf_document resource
* @param $fieldname string
* @param $value mixed
* @param $isName (optional) int
*/
function fdf_set_value($fdf_document, $fieldname, $value, $isName);
/**
* Sets version number for a FDF file
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $fdf_document resource
* @param $version string
*/
function fdf_set_version($fdf_document, $version);
/**
* Tests for end-of-file on a file pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function feof($handle);
/**
* Flushes the output to a file
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
* @param $handle resource
*/
function fflush($handle);
/**
* Gets character from file pointer
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function fgetc($handle);
/**
* Gets line from file pointer and parse for CSV fields
* @return array
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $handle resource
* @param $length (optional) int
* @param $delimiter (optional) string
* @param $enclosure (optional) string
*/
function fgetcsv($handle, $length, $delimiter, $enclosure);
/**
* Gets line from file pointer
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
* @param $length (optional) int
*/
function fgets($handle, $length);
/**
* Gets line from file pointer and strip HTML tags
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
* @param $length (optional) int
* @param $allowable_tags (optional) string
*/
function fgetss($handle, $length, $allowable_tags);
/**
* Reads entire file into an array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $use_include_path (optional) int
* @param $context (optional) resource
*/
function file($filename, $use_include_path, $context);
/**
* Gets last access time of file
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function fileatime($filename);
/**
* Gets inode change time of file
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function filectime($filename);
/**
* Gets file group
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function filegroup($filename);
/**
* Gets file inode
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function fileinode($filename);
/**
* Gets file modification time
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function filemtime($filename);
/**
* Gets file owner
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function fileowner($filename);
/**
* Gets file permissions
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function fileperms($filename);
/**
* Read and verify the map file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $directory string
*/
function filepro($directory);
/**
* Find out how many fields are in a filePro database
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function filepro_fieldcount();
/**
* Gets the name of a field
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $field_number int
*/
function filepro_fieldname($field_number);
/**
* Gets the type of a field
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $field_number int
*/
function filepro_fieldtype($field_number);
/**
* Gets the width of a field
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $field_number int
*/
function filepro_fieldwidth($field_number);
/**
* Retrieves data from a filePro database
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $row_number int
* @param $field_number int
*/
function filepro_retrieve($row_number, $field_number);
/**
* Find out how many rows are in a filePro database
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function filepro_rowcount();
/**
* Gets file size
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function filesize($filename);
/**
* Gets file type
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function filetype($filename);
/**
* Checks whether a file or directory exists
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function file_exists($filename);
/**
* Reads entire file into a string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $filename string
* @param $use_include_path (optional) bool
* @param $context (optional) resource
* @param $offset (optional) int
* @param $maxlen (optional) int
*/
function file_get_contents($filename, $use_include_path, $context, $offset, $maxlen);
/**
* Write a string to a file
* @return int
* @version PHP 5
* @param $filename string
* @param $data mixed
* @param $flags (optional) int
* @param $context (optional) resource
*/
function file_put_contents($filename, $data, $flags, $context);
/**
* Return information about a string buffer
* @return string
* @version PECL
* @param $finfo resource
* @param $string string
* @param $options (optional) int
*/
function finfo_buffer($finfo, $string, $options);
/**
* Close fileinfo resource
* @return bool
* @version PECL
* @param $finfo resource
*/
function finfo_close($finfo);
/**
* Return information about a file
* @return string
* @version PECL
* @param $finfo resource
* @param $file_name string
* @param $options (optional) int
* @param $context (optional) resource
*/
function finfo_file($finfo, $file_name, $options, $context);
/**
* Create a new fileinfo resource
* @return resource
* @version PECL
* @param $options int
* @param $arg (optional) string
*/
function finfo_open($options, $arg);
/**
* Set libmagic configuration options
* @return bool
* @version PECL
* @param $finfo resource
* @param $options int
*/
function finfo_set_flags($finfo, $options);
/**
* Get float value of a variable
* @return float
* @version PHP 4 >= 4.2.0, PHP 5
* @param $var mixed
*/
function floatval($var);
/**
* Portable advisory file locking
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $handle resource
* @param $operation int
* @param &$wouldblock (optional) int
*/
function flock($handle, $operation, &$wouldblock);
/**
* Round fractions down
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $value float
*/
function floor($value);
/**
* Flush the output buffer
* @return 
* @version PHP 3, PHP 4, PHP 5
*/
function flush();
/**
* Returns the floating point remainder (modulo) of the division of the arguments
* @return float
* @version PHP 4 >= 4.2.0, PHP 5
* @param $x float
* @param $y float
*/
function fmod($x, $y);
/**
* Match filename against a pattern
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $pattern string
* @param $string string
* @param $flags (optional) int
*/
function fnmatch($pattern, $string, $flags);
/**
* Opens file or URL
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $mode string
* @param $use_include_path (optional) bool
* @param $zcontext (optional) resource
*/
function fopen($filename, $mode, $use_include_path, $zcontext);
/**
* Output all remaining data on a file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function fpassthru($handle);
/**
* Write a formatted string to a stream
* @return int
* @version PHP 5
* @param $handle resource
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function fprintf($handle, $format, $args, $params1);
/**
* Format line as CSV and write to file pointer
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $handle resource
* @param $fields (optional) array
* @param $delimiter (optional) string
* @param $enclosure (optional) string
*/
function fputcsv($handle, $fields, $delimiter, $enclosure);
/**
* Alias of fwrite()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function fputs();
/**
* Binary-safe file read
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
* @param $length int
*/
function fread($handle, $length);
/**
* Converts a date from the French Republican Calendar to a Julian Day Count
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $month int
* @param $day int
* @param $year int
*/
function FrenchToJD($month, $day, $year);
/**
* Convert a logical string to a visual one
* @return string
* @version PHP 4 >= 4.0.4, PECL
* @param $str string
* @param $direction string
* @param $charset int
*/
function fribidi_log2vis($str, $direction, $charset);
/**
* Parses input from a file according to a format
* @return mixed
* @version PHP 4 >= 4.0.1, PHP 5
* @param $handle resource
* @param $format string
* @param &$... (optional) mixed
*/
function fscanf($handle, $format, &$...);
/**
* Seeks on a file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
* @param $offset int
* @param $whence (optional) int
*/
function fseek($handle, $offset, $whence);
/**
* Open Internet or Unix domain socket connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $target string
* @param $port (optional) int
* @param &$errno (optional) int
* @param &$errstr (optional) string
* @param $timeout (optional) float
*/
function fsockopen($target, $port, &$errno, &$errstr, $timeout);
/**
* Gets information about a file using an open file pointer
* @return array
* @version PHP 4, PHP 5
* @param $handle resource
*/
function fstat($handle);
/**
* Tells file pointer read/write position
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function ftell($handle);
/**
* Convert a pathname and a project identifier to a System V IPC key
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $pathname string
* @param $proj string
*/
function ftok($pathname, $proj);
/**
* Allocates space for a file to be uploaded
* @return bool
* @version PHP 5
* @param $ftp_stream resource
* @param $filesize int
* @param &$result (optional) string
*/
function ftp_alloc($ftp_stream, $filesize, &$result);
/**
* Changes to the parent directory
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
*/
function ftp_cdup($ftp_stream);
/**
* Changes the current directory on a FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $directory string
*/
function ftp_chdir($ftp_stream, $directory);
/**
* Set permissions on a file via FTP
* @return int
* @version PHP 5
* @param $ftp_stream resource
* @param $mode int
* @param $filename string
*/
function ftp_chmod($ftp_stream, $mode, $filename);
/**
* Closes an FTP connection
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $ftp_stream resource
*/
function ftp_close($ftp_stream);
/**
* Opens an FTP connection
* @return resource
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $host string
* @param $port (optional) int
* @param $timeout (optional) int
*/
function ftp_connect($host, $port, $timeout);
/**
* Deletes a file on the FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $path string
*/
function ftp_delete($ftp_stream, $path);
/**
* Requests execution of a command on the FTP server
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $ftp_stream resource
* @param $command string
*/
function ftp_exec($ftp_stream, $command);
/**
* Downloads a file from the FTP server and saves to an open file
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $handle resource
* @param $remote_file string
* @param $mode int
* @param $resumepos (optional) int
*/
function ftp_fget($ftp_stream, $handle, $remote_file, $mode, $resumepos);
/**
* Uploads from an open file to the FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
* @param $handle resource
* @param $mode int
* @param $startpos (optional) int
*/
function ftp_fput($ftp_stream, $remote_file, $handle, $mode, $startpos);
/**
* Downloads a file from the FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $local_file string
* @param $remote_file string
* @param $mode int
* @param $resumepos (optional) int
*/
function ftp_get($ftp_stream, $local_file, $remote_file, $mode, $resumepos);
/**
* Retrieves various runtime behaviours of the current FTP stream
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5
* @param $ftp_stream resource
* @param $option int
*/
function ftp_get_option($ftp_stream, $option);
/**
* Logs in to an FTP connection
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $username string
* @param $password string
*/
function ftp_login($ftp_stream, $username, $password);
/**
* Returns the last modified time of the given file
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
*/
function ftp_mdtm($ftp_stream, $remote_file);
/**
* Creates a directory
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $directory string
*/
function ftp_mkdir($ftp_stream, $directory);
/**
* Continues retrieving/sending a file (non-blocking)
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $ftp_stream resource
*/
function ftp_nb_continue($ftp_stream);
/**
* Retrieves a file from the FTP server and writes it to an open file (non-blocking)
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $ftp_stream resource
* @param $handle resource
* @param $remote_file string
* @param $mode int
* @param $resumepos (optional) int
*/
function ftp_nb_fget($ftp_stream, $handle, $remote_file, $mode, $resumepos);
/**
* Stores a file from an open file to the FTP server (non-blocking)
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
* @param $handle resource
* @param $mode int
* @param $startpos (optional) int
*/
function ftp_nb_fput($ftp_stream, $remote_file, $handle, $mode, $startpos);
/**
* Retrieves a file from the FTP server and writes it to a local file (non-blocking)
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $ftp_stream resource
* @param $local_file string
* @param $remote_file string
* @param $mode int
* @param $resumepos (optional) int
*/
function ftp_nb_get($ftp_stream, $local_file, $remote_file, $mode, $resumepos);
/**
* Stores a file on the FTP server (non-blocking)
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
* @param $local_file string
* @param $mode int
* @param $startpos (optional) int
*/
function ftp_nb_put($ftp_stream, $remote_file, $local_file, $mode, $startpos);
/**
* Returns a list of files in the given directory
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $directory string
*/
function ftp_nlist($ftp_stream, $directory);
/**
* Turns passive mode on or off
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $pasv bool
*/
function ftp_pasv($ftp_stream, $pasv);
/**
* Uploads a file to the FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
* @param $local_file string
* @param $mode int
* @param $startpos (optional) int
*/
function ftp_put($ftp_stream, $remote_file, $local_file, $mode, $startpos);
/**
* Returns the current directory name
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
*/
function ftp_pwd($ftp_stream);
/**
* Alias of ftp_close()
* @return &#13;
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function ftp_quit();
/**
* Sends an arbitrary command to an FTP server
* @return array
* @version PHP 5
* @param $ftp_stream resource
* @param $command string
*/
function ftp_raw($ftp_stream, $command);
/**
* Returns a detailed list of files in the given directory
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $directory string
* @param $recursive (optional) bool
*/
function ftp_rawlist($ftp_stream, $directory, $recursive);
/**
* Renames a file or a directory on the FTP server
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $oldname string
* @param $newname string
*/
function ftp_rename($ftp_stream, $oldname, $newname);
/**
* Removes a directory
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $directory string
*/
function ftp_rmdir($ftp_stream, $directory);
/**
* Set miscellaneous runtime FTP options
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $ftp_stream resource
* @param $option int
* @param $value mixed
*/
function ftp_set_option($ftp_stream, $option, $value);
/**
* Sends a SITE command to the server
* @return bool
* @version PHP 3 >= 3.0.15, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $command string
*/
function ftp_site($ftp_stream, $command);
/**
* Returns the size of the given file
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
* @param $remote_file string
*/
function ftp_size($ftp_stream, $remote_file);
/**
* Opens an Secure SSL-FTP connection
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $host string
* @param $port (optional) int
* @param $timeout (optional) int
*/
function ftp_ssl_connect($host, $port, $timeout);
/**
* Returns the system type identifier of the remote FTP server
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $ftp_stream resource
*/
function ftp_systype($ftp_stream);
/**
* Truncates a file to a given length
* @return bool
* @version PHP 4, PHP 5
* @param $handle resource
* @param $size int
*/
function ftruncate($handle, $size);
/**
* Return TRUE if the given function has been defined
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $function_name string
*/
function function_exists($function_name);
/**
* Return an item from the argument list
* @return mixed
* @version PHP 4, PHP 5
* @param $arg_num int
*/
function func_get_arg($arg_num);
/**
* Returns an array comprising a function's argument list
* @return array
* @version PHP 4, PHP 5
*/
function func_get_args();
/**
* Returns the number of arguments passed to the function
* @return int
* @version PHP 4, PHP 5
*/
function func_num_args();
/**
* Binary-safe file write
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
* @param $string string
* @param $length (optional) int
*/
function fwrite($handle, $string, $length);
/**
* Retrieve information about the currently installed GD library
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function gd_info();
/**
* Fetch all HTTP request headers
* @return array
* @version PHP 3, PHP 4, PHP 5
*/
function getallheaders();
/**
* Gets the current working directory
* @return string
* @version PHP 4, PHP 5
*/
function getcwd();
/**
* Get date/time information
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $timestamp int
*/
function getdate($timestamp);
/**
* Gets the value of an environment variable
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $varname string
*/
function getenv($varname);
/**
* Get the Internet host name corresponding to a given IP address
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $ip_address string
*/
function gethostbyaddr($ip_address);
/**
* Get the IP address corresponding to a given Internet host name
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
*/
function gethostbyname($hostname);
/**
* Get a list of IP addresses corresponding to a given Internet host name
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
*/
function gethostbynamel($hostname);
/**
* Get the size of an image
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param &$imageinfo (optional) array
*/
function getimagesize($filename, &$imageinfo);
/**
* Gets time of last page modification
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function getlastmod();
/**
* Get MX records corresponding to a given Internet host name
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
* @param &$mxhosts array
* @param &$weight (optional) array
*/
function getmxrr($hostname, &$mxhosts, &$weight);
/**
* Get PHP script owner's GID
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function getmygid();
/**
* Gets the inode of the current script
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function getmyinode();
/**
* Gets PHP's process ID
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function getmypid();
/**
* Gets PHP script owner's UID
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function getmyuid();
/**
* Gets options from the command line argument list
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $options string
*/
function getopt($options);
/**
* Get protocol number associated with protocol name
* @return int
* @version PHP 4, PHP 5
* @param $name string
*/
function getprotobyname($name);
/**
* Get protocol name associated with protocol number
* @return string
* @version PHP 4, PHP 5
* @param $number int
*/
function getprotobynumber($number);
/**
* Show largest possible random value
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function getrandmax();
/**
* Gets the current resource usages
* @return array
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $who int
*/
function getrusage($who);
/**
* Get port number associated with an Internet service and protocol
* @return int
* @version PHP 4, PHP 5
* @param $service string
* @param $protocol string
*/
function getservbyname($service, $protocol);
/**
* Get Internet service which corresponds to port and protocol
* @return string
* @version PHP 4, PHP 5
* @param $port int
* @param $protocol string
*/
function getservbyport($port, $protocol);
/**
* Lookup a message in the current domain
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $message string
*/
function gettext($message);
/**
* Get current time
* @return mixed
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $return_float bool
*/
function gettimeofday($return_float);
/**
* Get the type of a variable
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function gettype($var);
/**
* Tells what the user's browser is capable of
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $user_agent string
* @param $return_array (optional) bool
*/
function get_browser($user_agent, $return_array);
/**
* Gets the value of a PHP configuration option
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $varname string
*/
function get_cfg_var($varname);
/**
* Returns the name of the class of an object
* @return string
* @version PHP 4, PHP 5
* @param $obj object
*/
function get_class($obj);
/**
* Returns an array of class methods' names
* @return array
* @version PHP 4, PHP 5
* @param $class_name mixed
*/
function get_class_methods($class_name);
/**
* Returns an array of default properties of the class
* @return array
* @version PHP 4, PHP 5
* @param $class_name string
*/
function get_class_vars($class_name);
/**
* Gets the name of the owner of the current PHP script
* @return string
* @version PHP 3, PHP 4, PHP 5
*/
function get_current_user();
/**
* Returns an array with the name of the defined classes
* @return array
* @version PHP 4, PHP 5
*/
function get_declared_classes();
/**
* Returns an array of all declared interfaces
* @return array
* @version PHP 5
*/
function get_declared_interfaces();
/**
* Returns an associative array with the names of all the constants and their values
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $categorize mixed
*/
function get_defined_constants($categorize);
/**
* Returns an array of all defined functions
* @return array
* @version PHP 4 >= 4.0.4, PHP 5
*/
function get_defined_functions();
/**
* Returns an array of all defined variables
* @return array
* @version PHP 4 >= 4.0.4, PHP 5
*/
function get_defined_vars();
/**
* Returns an array with the names of the functions of a module
* @return array
* @version PHP 4, PHP 5
* @param $module_name string
*/
function get_extension_funcs($module_name);
/**
* Fetches all the headers sent by the server in response to a HTTP request
* @return array
* @version PHP 5
* @param $url string
* @param $format (optional) int
*/
function get_headers($url, $format);
/**
* Returns the translation table used by htmlspecialchars() and htmlentities()
* @return array
* @version PHP 4, PHP 5
* @param $table int
* @param $quote_style (optional) int
*/
function get_html_translation_table($table, $quote_style);
/**
* Returns an array with the names of included or required files
* @return array
* @version PHP 4, PHP 5
*/
function get_included_files();
/**
* Gets the current include_path configuration option
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
*/
function get_include_path();
/**
* Returns an array with the names of all modules compiled and loaded
* @return array
* @version PHP 4, PHP 5
*/
function get_loaded_extensions();
/**
* Gets the current configuration setting of magic quotes gpc
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
*/
function get_magic_quotes_gpc();
/**
* Gets the current active configuration setting of magic_quotes_runtime
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
*/
function get_magic_quotes_runtime();
/**
* Extracts all meta tag content attributes from a file and returns an array
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $filename string
* @param $use_include_path (optional) bool
*/
function get_meta_tags($filename, $use_include_path);
/**
* Returns an associative array of object properties
* @return array
* @version PHP 4, PHP 5
* @param $obj object
*/
function get_object_vars($obj);
/**
* Retrieves the parent class name for object or class
* @return string
* @version PHP 4, PHP 5
* @param $obj mixed
*/
function get_parent_class($obj);
/**
* Alias of get_included_files()
* @return &#13;
* @version PHP 4, PHP 5
*/
function get_required_files();
/**
* Returns the resource type
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $handle resource
*/
function get_resource_type($handle);
/**
* Find pathnames matching a pattern
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $pattern string
* @param $flags (optional) int
*/
function glob($pattern, $flags);
/**
* Format a GMT/UTC date/time
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $timestamp (optional) int
*/
function gmdate($format, $timestamp);
/**
* Get Unix timestamp for a GMT date
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $hour int
* @param $minute (optional) int
* @param $second (optional) int
* @param $month (optional) int
* @param $day (optional) int
* @param $year (optional) int
* @param $is_dst (optional) int
*/
function gmmktime($hour, $minute, $second, $month, $day, $year, $is_dst);
/**
* Absolute value
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_abs($a);
/**
* Add numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_add($a, $b);
/**
* Logical AND
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_and($a, $b);
/**
* Clear bit
* @return 
* @version PHP 4 >= 4.0.4, PHP 5
* @param &$a resource
* @param $index int
*/
function gmp_clrbit(&$a, $index);
/**
* Compare numbers
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_cmp($a, $b);
/**
* Calculates one's complement
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_com($a);
/**
* Alias of gmp_div_q()
* @return &#13;
* @version PHP 4 >= 4.0.4, PHP 5
*/
function gmp_div();
/**
* Exact division of numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $n resource
* @param $d resource
*/
function gmp_divexact($n, $d);
/**
* Divide numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
* @param $round (optional) int
*/
function gmp_div_q($a, $b, $round);
/**
* Divide numbers and get quotient and remainder
* @return array
* @version PHP 4 >= 4.0.4, PHP 5
* @param $n resource
* @param $d resource
* @param $round (optional) int
*/
function gmp_div_qr($n, $d, $round);
/**
* Remainder of the division of numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $n resource
* @param $d resource
* @param $round (optional) int
*/
function gmp_div_r($n, $d, $round);
/**
* Factorial
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a int
*/
function gmp_fact($a);
/**
* Calculate GCD
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_gcd($a, $b);
/**
* Calculate GCD and multipliers
* @return array
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_gcdext($a, $b);
/**
* Hamming distance
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_hamdist($a, $b);
/**
* Create GMP number
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $number mixed
* @param $base (optional) int
*/
function gmp_init($number, $base);
/**
* Convert GMP number to integer
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $gmpnumber resource
*/
function gmp_intval($gmpnumber);
/**
* Inverse by modulo
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_invert($a, $b);
/**
* Jacobi symbol
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $p resource
*/
function gmp_jacobi($a, $p);
/**
* Legendre symbol
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $p resource
*/
function gmp_legendre($a, $p);
/**
* Modulo operation
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $n resource
* @param $d resource
*/
function gmp_mod($n, $d);
/**
* Multiply numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_mul($a, $b);
/**
* Negate number
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_neg($a);
/**
* Logical OR
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_or($a, $b);
/**
* Perfect square check
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_perfect_square($a);
/**
* Population count
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_popcount($a);
/**
* Raise number into power
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $base resource
* @param $exp int
*/
function gmp_pow($base, $exp);
/**
* Raise number into power with modulo
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $base resource
* @param $exp resource
* @param $mod resource
*/
function gmp_powm($base, $exp, $mod);
/**
* Check if number is "probably prime"
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $reps (optional) int
*/
function gmp_prob_prime($a, $reps);
/**
* Random number
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $limiter int
*/
function gmp_random($limiter);
/**
* Scan for 0
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $start int
*/
function gmp_scan0($a, $start);
/**
* Scan for 1
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $start int
*/
function gmp_scan1($a, $start);
/**
* Set bit
* @return 
* @version PHP 4 >= 4.0.4, PHP 5
* @param &$a resource
* @param $index int
* @param $set_clear (optional) bool
*/
function gmp_setbit(&$a, $index, $set_clear);
/**
* Sign of number
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_sign($a);
/**
* Calculate square root
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_sqrt($a);
/**
* Square root with remainder
* @return array
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
*/
function gmp_sqrtrem($a);
/**
* Convert GMP number to string
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $gmpnumber resource
* @param $base (optional) int
*/
function gmp_strval($gmpnumber, $base);
/**
* Subtract numbers
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_sub($a, $b);
/**
* Logical XOR
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $a resource
* @param $b resource
*/
function gmp_xor($a, $b);
/**
* Format a GMT/UTC time/date according to locale settings
* @return string
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $format string
* @param $timestamp (optional) int
*/
function gmstrftime($format, $timestamp);
/**
* Translate a gopher formatted directory entry into an associative array.
* @return array
* @version PECL
* @param $dirent string
*/
function gopher_parsedir($dirent);
/**
* Converts a Gregorian date to Julian Day Count
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $month int
* @param $day int
* @param $year int
*/
function GregorianToJD($month, $day, $year);
/**
* Close an open gz-file pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gzclose($zp);
/**
* Compress a string
* @return string
* @version PHP 4 >= 4.0.1, PHP 5
* @param $data string
* @param $level (optional) int
*/
function gzcompress($data, $level);
/**
* Deflate a string
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param $level (optional) int
*/
function gzdeflate($data, $level);
/**
* Create a gzip compressed string
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param $level (optional) int
* @param $encoding_mode (optional) int
*/
function gzencode($data, $level, $encoding_mode);
/**
* Test for end-of-file on a gz-file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gzeof($zp);
/**
* Read entire gz-file into an array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $use_include_path (optional) int
*/
function gzfile($filename, $use_include_path);
/**
* Get character from gz-file pointer
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gzgetc($zp);
/**
* Get line from file pointer
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
* @param $length int
*/
function gzgets($zp, $length);
/**
* Get line from gz-file pointer and strip HTML tags
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
* @param $length int
* @param $allowable_tags (optional) string
*/
function gzgetss($zp, $length, $allowable_tags);
/**
* Inflate a deflated string
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param $length (optional) int
*/
function gzinflate($data, $length);
/**
* Open gz-file
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $mode string
* @param $use_include_path (optional) int
*/
function gzopen($filename, $mode, $use_include_path);
/**
* Output all remaining data on a gz-file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gzpassthru($zp);
/**
* Alias of gzwrite()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function gzputs();
/**
* Binary-safe gz-file read
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
* @param $length int
*/
function gzread($zp, $length);
/**
* Rewind the position of a gz-file pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gzrewind($zp);
/**
* Seek on a gz-file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
* @param $offset int
*/
function gzseek($zp, $offset);
/**
* Tell gz-file pointer read/write position
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
*/
function gztell($zp);
/**
* Uncompress a compressed string
* @return string
* @version PHP 4 >= 4.0.1, PHP 5
* @param $data string
* @param $length (optional) int
*/
function gzuncompress($data, $length);
/**
* Binary-safe gz-file write
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $zp resource
* @param $string string
* @param $length (optional) int
*/
function gzwrite($zp, $string, $length);
/**
* Send a raw HTTP header
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $string string
* @param $replace (optional) bool
* @param $http_response_code (optional) int
*/
function header($string, $replace, $http_response_code);
/**
* Returns a list of response headers sent (or ready to send)
* @return array
* @version PHP 5
*/
function headers_list();
/**
* Checks if or where headers have been sent
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param &$file string
* @param &$line (optional) int
*/
function headers_sent(&$file, &$line);
/**
* Convert logical Hebrew text to visual text
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $hebrew_text string
* @param $max_chars_per_line (optional) int
*/
function hebrev($hebrew_text, $max_chars_per_line);
/**
* Convert logical Hebrew text to visual text with newline conversion
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $hebrew_text string
* @param $max_chars_per_line (optional) int
*/
function hebrevc($hebrew_text, $max_chars_per_line);
/**
* Hexadecimal to decimal
* @return number
* @version PHP 3, PHP 4, PHP 5
* @param $hex_string string
*/
function hexdec($hex_string);
/**
* Syntax highlighting of a file
* @return mixed
* @version PHP 4, PHP 5
* @param $filename string
* @param $return (optional) bool
*/
function highlight_file($filename, $return);
/**
* Syntax highlighting of a string
* @return mixed
* @version PHP 4, PHP 5
* @param $str string
* @param $return (optional) bool
*/
function highlight_string($str, $return);
/**
* Convert all applicable characters to HTML entities
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
* @param $quote_style (optional) int
* @param $charset (optional) string
*/
function htmlentities($string, $quote_style, $charset);
/**
* Convert special characters to HTML entities
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
* @param $quote_style (optional) int
* @param $charset (optional) string
*/
function htmlspecialchars($string, $quote_style, $charset);
/**
* Convert special HTML entities back to characters
* @return string
* @version PHP 5 >= 5.1.0RC1
* @param $string string
* @param $quote_style (optional) int
*/
function htmlspecialchars_decode($string, $quote_style);
/**
* Convert all HTML entities to their applicable characters
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $string string
* @param $quote_style (optional) int
* @param $charset (optional) string
*/
function html_entity_decode($string, $quote_style, $charset);
/**
* Generate URL-encoded query string
* @return string
* @version PHP 5
* @param $formdata array
* @param $numeric_prefix (optional) string
*/
function http_build_query($formdata, $numeric_prefix);
/**
* Caching by ETag
* @return bool
* @version PECL
* @param $etag string
*/
function http_cache_etag($etag);
/**
* Caching by last modification
* @return bool
* @version PECL
* @param $timestamp_or_expires int
*/
function http_cache_last_modified($timestamp_or_expires);
/**
* Decode chunked-encoded data
* @return string
* @version PECL
* @param $encoded string
*/
function http_chunked_decode($encoded);
/**
* Compose HTTP RFC compliant date
* @return string
* @version PECL
* @param $timestamp int
*/
function http_date($timestamp);
/**
* Perform GET request
* @return string
* @version PECL
* @param $url string
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_get($url, $options, &$info);
/**
* Get request body as string
* @return string
* @version PECL
*/
function http_get_request_body();
/**
* Get request headers as array
* @return array
* @version PECL
*/
function http_get_request_headers();
/**
* Perform HEAD request
* @return string
* @version PECL
* @param $url string
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_head($url, $options, &$info);
/**
* Match ETag
* @return bool
* @version PECL
* @param $etag string
* @param $for_range (optional) bool
*/
function http_match_etag($etag, $for_range);
/**
* Match last modification
* @return bool
* @version PECL
* @param $timestamp int
* @param $for_range (optional) bool
*/
function http_match_modified($timestamp, $for_range);
/**
* Match any header
* @return bool
* @version PECL
* @param $header string
* @param $value (optional) string
* @param $match_case (optional) bool
*/
function http_match_request_header($header, $value, $match_case);
/**
* Negotiate clients preferred character set
* @return string
* @version PECL
* @param $supported array
* @param &$result (optional) array
*/
function http_negotiate_charset($supported, &$result);
/**
* Negotiate clients preferred language
* @return string
* @version PECL
* @param $supported array
* @param &$result (optional) array
*/
function http_negotiate_language($supported, &$result);
/**
* Parse HTTP headers
* @return array
* @version PECL
* @param $header string
*/
function http_parse_headers($header);
/**
* Parse HTTP message
* @return object
* @version PECL
* @param $message string
*/
function http_parse_message($message);
/**
* Perform POST request with pre-encoded data
* @return string
* @version PECL
* @param $url string
* @param $data (optional) string
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_post_data($url, $data, $options, &$info);
/**
* Perform POST request with data to be encoded
* @return string
* @version PECL
* @param $url string
* @param $data (optional) array
* @param $files (optional) array
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_post_fields($url, $data, $files, $options, &$info);
/**
* Perform PUT request with file
* @return string
* @version PECL
* @param $url string
* @param $file (optional) string
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_put_file($url, $file, $options, &$info);
/**
* Perform PUT request with stream
* @return string
* @version PECL
* @param $url string
* @param $stream (optional) resource
* @param $options (optional) array
* @param &$info (optional) array
*/
function http_put_stream($url, $stream, $options, &$info);
/**
* Issue HTTP redirect
* @return 
* @version PECL
* @param $url string
* @param $params (optional) array
* @param $session (optional) bool
* @param $status (optional) int
*/
function http_redirect($url, $params, $session, $status);
/**
* Check whether request method exists
* @return int
* @version PECL
* @param $method mixed
*/
function http_request_method_exists($method);
/**
* Get request method name
* @return string
* @version PECL
* @param $method int
*/
function http_request_method_name($method);
/**
* Register request method
* @return int
* @version PECL
* @param $method string
*/
function http_request_method_register($method);
/**
* Unregister request method
* @return bool
* @version PECL
* @param $method mixed
*/
function http_request_method_unregister($method);
/**
* Send Content-Disposition
* @return bool
* @version PECL
* @param $filename string
* @param $inline (optional) bool
*/
function http_send_content_disposition($filename, $inline);
/**
* Send Content-Type
* @return bool
* @version PECL
* @param $content_type string
*/
function http_send_content_type($content_type);
/**
* Send arbitrary data
* @return bool
* @version PECL
* @param $data string
*/
function http_send_data($data);
/**
* Send file
* @return bool
* @version PECL
* @param $file string
*/
function http_send_file($file);
/**
* Send Last-Modified
* @return bool
* @version PECL
* @param $timestamp int
*/
function http_send_last_modified($timestamp);
/**
* Send status
* @return bool
* @version PECL
* @param $status int
*/
function http_send_status($status);
/**
* Send stream
* @return bool
* @version PECL
* @param $stream resource
*/
function http_send_stream($stream);
/**
* HTTP throttling
* @return 
* @version PECL
* @param $sec double
* @param $bytes (optional) int
*/
function http_throttle($sec, $bytes);
/**
* Convert attributes from object array to object record
* @return string
* @version PHP 3 >= 3.0.4, PHP 4, PECL
* @param $object_array array
*/
function hw_Array2Objrec($object_array);
/**
* Changes attributes of an object (obsolete)
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
* @param $objid int
* @param $attributes array
*/
function hw_changeobject($link, $objid, $attributes);
/**
* Object ids of children
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_Children($connection, $objectID);
/**
* Object records of children
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_ChildrenObj($connection, $objectID);
/**
* Closes the Hyperwave connection
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_Close($connection);
/**
* Opens a connection
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $host string
* @param $port int
* @param $username (optional) string
* @param $password (optional) string
*/
function hw_Connect($host, $port, $username, $password);
/**
* Prints information about the connection to Hyperwave server
* @return 
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
*/
function hw_connection_info($link);
/**
* Copies objects
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_id_array array
* @param $destination_id int
*/
function hw_cp($connection, $object_id_array, $destination_id);
/**
* Deletes object
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_to_delete int
*/
function hw_Deleteobject($connection, $object_to_delete);
/**
* Object id object belonging to anchor
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $anchorID int
*/
function hw_DocByAnchor($connection, $anchorID);
/**
* Object record object belonging to anchor
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $anchorID int
*/
function hw_DocByAnchorObj($connection, $anchorID);
/**
* Object record of hw_document
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
*/
function hw_Document_Attributes($hw_document);
/**
* Body tag of hw_document
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
* @param $prefix (optional) string
*/
function hw_Document_BodyTag($hw_document, $prefix);
/**
* Returns content of hw_document
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
*/
function hw_Document_Content($hw_document);
/**
* Sets/replaces content of hw_document
* @return bool
* @version PHP 4, PECL
* @param $hw_document int
* @param $content string
*/
function hw_Document_SetContent($hw_document, $content);
/**
* Size of hw_document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
*/
function hw_Document_Size($hw_document);
/**
* Hyperwave dummy function
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
* @param $id int
* @param $msgid int
*/
function hw_dummy($link, $id, $msgid);
/**
* Retrieve text document
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $hw_document int
*/
function hw_EditText($connection, $hw_document);
/**
* Error number
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_Error($connection);
/**
* Returns error message
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_ErrorMsg($connection);
/**
* Frees hw_document
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
*/
function hw_Free_Document($hw_document);
/**
* Object ids of anchors of document
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetAnchors($connection, $objectID);
/**
* Object records of anchors of document
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetAnchorsObj($connection, $objectID);
/**
* Return object record and lock object
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetAndLock($connection, $objectID);
/**
* Object ids of child collections
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetChildColl($connection, $objectID);
/**
* Object records of child collections
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetChildCollObj($connection, $objectID);
/**
* Object ids of child documents of collection
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetChildDocColl($connection, $objectID);
/**
* Object records of child documents of collection
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetChildDocCollObj($connection, $objectID);
/**
* Object record
* @return mixed
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID mixed
* @param $query (optional) string
*/
function hw_GetObject($connection, $objectID, $query);
/**
* Search object
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $query string
* @param $max_hits int
*/
function hw_GetObjectByQuery($connection, $query, $max_hits);
/**
* Search object in collection
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
* @param $query string
* @param $max_hits int
*/
function hw_GetObjectByQueryColl($connection, $objectID, $query, $max_hits);
/**
* Search object in collection
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
* @param $query string
* @param $max_hits int
*/
function hw_GetObjectByQueryCollObj($connection, $objectID, $query, $max_hits);
/**
* Search object
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $query string
* @param $max_hits int
*/
function hw_GetObjectByQueryObj($connection, $query, $max_hits);
/**
* Object ids of parents
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetParents($connection, $objectID);
/**
* Object records of parents
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetParentsObj($connection, $objectID);
/**
* Get link from source to dest relative to rootid
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
* @param $rootid int
* @param $sourceid int
* @param $destid int
*/
function hw_getrellink($link, $rootid, $sourceid, $destid);
/**
* Gets a remote document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetRemote($connection, $objectID);
/**
* Gets children of remote document
* @return mixed
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_record string
*/
function hw_getremotechildren($connection, $object_record);
/**
* Returns anchors pointing at object
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_GetSrcByDestObj($connection, $objectID);
/**
* Retrieve text document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
* @param $rootID/prefix (optional) mixed
*/
function hw_GetText($connection, $objectID, $rootID/prefix);
/**
* Name of currently logged in user
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_getusername($connection);
/**
* Identifies as user
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
* @param $username string
* @param $password string
*/
function hw_Identify($link, $username, $password);
/**
* Check if object ids in collections
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_id_array array
* @param $collection_id_array array
* @param $return_collections int
*/
function hw_InCollections($connection, $object_id_array, $collection_id_array, $return_collections);
/**
* Info about connection
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_Info($connection);
/**
* Insert collection
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
* @param $object_array array
*/
function hw_InsColl($connection, $objectID, $object_array);
/**
* Insert document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection resource
* @param $parentID int
* @param $object_record string
* @param $text (optional) string
*/
function hw_InsDoc($connection, $parentID, $object_record, $text);
/**
* Inserts only anchors into text
* @return bool
* @version PHP 4 >= 4.0.4, PECL
* @param $hwdoc int
* @param $anchorecs array
* @param $dest array
* @param $urlprefixes (optional) array
*/
function hw_insertanchors($hwdoc, $anchorecs, $dest, $urlprefixes);
/**
* Upload any document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $parent_id int
* @param $hw_document int
*/
function hw_InsertDocument($connection, $parent_id, $hw_document);
/**
* Inserts an object record
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_rec string
* @param $parameter string
*/
function hw_InsertObject($connection, $object_rec, $parameter);
/**
* Maps global id on virtual local id
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $connection int
* @param $server_id int
* @param $object_id int
*/
function hw_mapid($connection, $server_id, $object_id);
/**
* Modifies object record
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PECL
* @param $connection int
* @param $object_to_change int
* @param $remove array
* @param $add array
* @param $mode (optional) int
*/
function hw_Modifyobject($connection, $object_to_change, $remove, $add, $mode);
/**
* Moves objects
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $object_id_array array
* @param $source_id int
* @param $destination_id int
*/
function hw_mv($connection, $object_id_array, $source_id, $destination_id);
/**
* Create new document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $object_record string
* @param $document_data string
* @param $document_size int
*/
function hw_New_Document($object_record, $document_data, $document_size);
/**
* Convert attributes from object record to object array
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $object_record string
* @param $format (optional) array
*/
function hw_objrec2array($object_record, $format);
/**
* Prints hw_document
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $hw_document int
*/
function hw_Output_Document($hw_document);
/**
* Make a persistent database connection
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $host string
* @param $port int
* @param $username (optional) string
* @param $password (optional) string
*/
function hw_pConnect($host, $port, $username, $password);
/**
* Retrieve any document
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
* @param $url_prefixes (optional) array
*/
function hw_PipeDocument($connection, $objectID, $url_prefixes);
/**
* Root object id
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
*/
function hw_Root();
/**
* Set the id to which links are calculated
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
* @param $rootid int
*/
function hw_setlinkroot($link, $rootid);
/**
* Returns status string
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $link int
*/
function hw_stat($link);
/**
* Unlock object
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
* @param $objectID int
*/
function hw_Unlock($connection, $objectID);
/**
* List of currently logged in users
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PECL
* @param $connection int
*/
function hw_Who($connection);
/**
* Calculate the length of the hypotenuse of a right-angle triangle
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $x float
* @param $y float
*/
function hypot($x, $y);
/**
* Add a user to a security database (only for IB6 or later)
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $service_handle resource
* @param $user_name string
* @param $password string
* @param $first_name (optional) string
* @param $middle_name (optional) string
* @param $last_name (optional) string
*/
function ibase_add_user($service_handle, $user_name, $password, $first_name, $middle_name, $last_name);
/**
* Return the number of rows that were affected by the previous query
* @return int
* @version PHP 5
* @param $link_identifier resource
*/
function ibase_affected_rows($link_identifier);
/**
* Initiates a backup task in the service manager and returns immediately
* @return mixed
* @version PHP 5
* @param $service_handle resource
* @param $source_db string
* @param $dest_file string
* @param $options (optional) int
* @param $verbose (optional) bool
*/
function ibase_backup($service_handle, $source_db, $dest_file, $options, $verbose);
/**
* Add data into a newly created blob
* @return 
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $blob_handle resource
* @param $data string
*/
function ibase_blob_add($blob_handle, $data);
/**
* Cancel creating blob
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $blob_handle resource
*/
function ibase_blob_cancel($blob_handle);
/**
* Close blob
* @return mixed
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $blob_handle resource
*/
function ibase_blob_close($blob_handle);
/**
* Create a new blob for adding data
* @return resource
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_identifier resource
*/
function ibase_blob_create($link_identifier);
/**
* Output blob contents to browser
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_identifier resource
* @param $blob_id string
*/
function ibase_blob_echo($link_identifier, $blob_id);
/**
* Get len bytes data from open blob
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $blob_handle resource
* @param $len int
*/
function ibase_blob_get($blob_handle, $len);
/**
* Create blob, copy file in it, and close it
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_identifier resource
* @param $file_handle resource
*/
function ibase_blob_import($link_identifier, $file_handle);
/**
* Return blob length and other useful info
* @return array
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_identifier resource
* @param $blob_id string
*/
function ibase_blob_info($link_identifier, $blob_id);
/**
* Open blob for retrieving data parts
* @return resource
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_identifier resource
* @param $blob_id string
*/
function ibase_blob_open($link_identifier, $blob_id);
/**
* Close a connection to an InterBase database
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
*/
function ibase_close($connection_id);
/**
* Commit a transaction
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_or_trans_identifier resource
*/
function ibase_commit($link_or_trans_identifier);
/**
* Commit a transaction without closing it
* @return bool
* @version PHP 5
* @param $link_or_trans_identifier resource
*/
function ibase_commit_ret($link_or_trans_identifier);
/**
* Open a connection to an InterBase database
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $database string
* @param $username (optional) string
* @param $password (optional) string
* @param $charset (optional) string
* @param $buffers (optional) int
* @param $dialect (optional) int
* @param $role (optional) string
* @param $sync (optional) int
*/
function ibase_connect($database, $username, $password, $charset, $buffers, $dialect, $role, $sync);
/**
* Request statistics about a database
* @return string
* @version PHP 5
* @param $service_handle resource
* @param $db string
* @param $action int
* @param $argument (optional) int
*/
function ibase_db_info($service_handle, $db, $action, $argument);
/**
* Delete a user from a security database (only for IB6 or later)
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $service_handle resource
* @param $user_name string
*/
function ibase_delete_user($service_handle, $user_name);
/**
* Drops a database
* @return bool
* @version PHP 5
* @param $connection resource
*/
function ibase_drop_db($connection);
/**
* Return an error code
* @return int
* @version PHP 5
*/
function ibase_errcode();
/**
* Return error messages
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ibase_errmsg();
/**
* Execute a previously prepared query
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $query resource
* @param $bind_arg (optional) mixed
* @param $params1 (optional) mixed
*/
function ibase_execute($query, $bind_arg, $params1);
/**
* Fetch a result row from a query as an associative array
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $result resource
* @param $fetch_flag (optional) int
*/
function ibase_fetch_assoc($result, $fetch_flag);
/**
* Get an object from a InterBase database
* @return object
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result_id resource
* @param $fetch_flag (optional) int
*/
function ibase_fetch_object($result_id, $fetch_flag);
/**
* Fetch a row from an InterBase database
* @return array
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_identifier resource
* @param $fetch_flag (optional) int
*/
function ibase_fetch_row($result_identifier, $fetch_flag);
/**
* Get information about a field
* @return array
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_number int
*/
function ibase_field_info($result, $field_number);
/**
* Cancels a registered event handler
* @return bool
* @version PHP 5
* @param $event resource
*/
function ibase_free_event_handler($event);
/**
* Free memory allocated by a prepared query
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $query resource
*/
function ibase_free_query($query);
/**
* Free a result set
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_identifier resource
*/
function ibase_free_result($result_identifier);
/**
* Increments the named generator and returns its new value
* @return mixed
* @version PHP 5
* @param $generator string
* @param $increment (optional) int
* @param $link_identifier (optional) resource
*/
function ibase_gen_id($generator, $increment, $link_identifier);
/**
* Execute a maintenance command on the database server
* @return bool
* @version PHP 5
* @param $service_handle resource
* @param $db string
* @param $action int
* @param $argument (optional) int
*/
function ibase_maintain_db($service_handle, $db, $action, $argument);
/**
* Modify a user to a security database (only for IB6 or later)
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $service_handle resource
* @param $user_name string
* @param $password string
* @param $first_name (optional) string
* @param $middle_name (optional) string
* @param $last_name (optional) string
*/
function ibase_modify_user($service_handle, $user_name, $password, $first_name, $middle_name, $last_name);
/**
* Assigns a name to a result set
* @return bool
* @version PHP 5
* @param $result resource
* @param $name string
*/
function ibase_name_result($result, $name);
/**
* Get the number of fields in a result set
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result_id resource
*/
function ibase_num_fields($result_id);
/**
* Return the number of parameters in a prepared query
* @return int
* @version PHP 5
* @param $query resource
*/
function ibase_num_params($query);
/**
* Return information about a parameter in a prepared query
* @return array
* @version PHP 5
* @param $query resource
* @param $param_number int
*/
function ibase_param_info($query, $param_number);
/**
* Open a persistent connection to an InterBase database
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $database string
* @param $username (optional) string
* @param $password (optional) string
* @param $charset (optional) string
* @param $buffers (optional) int
* @param $dialect (optional) int
* @param $role (optional) string
* @param $sync (optional) int
*/
function ibase_pconnect($database, $username, $password, $charset, $buffers, $dialect, $role, $sync);
/**
* Prepare a query for later binding of parameter placeholders and execution
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $query string
*/
function ibase_prepare($query);
/**
* Execute a query on an InterBase database
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $link_identifier resource
* @param $query (optional) string
* @param $bind_args (optional) int
*/
function ibase_query($link_identifier, $query, $bind_args);
/**
* Initiates a restore task in the service manager and returns immediately
* @return mixed
* @version PHP 5
* @param $service_handle resource
* @param $source_file string
* @param $dest_db string
* @param $options (optional) int
* @param $verbose (optional) bool
*/
function ibase_restore($service_handle, $source_file, $dest_db, $options, $verbose);
/**
* Roll back a transaction
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $link_or_trans_identifier resource
*/
function ibase_rollback($link_or_trans_identifier);
/**
* Roll back a transaction without closing it
* @return bool
* @version PHP 5
* @param $link_or_trans_identifier resource
*/
function ibase_rollback_ret($link_or_trans_identifier);
/**
* Request information about a database server
* @return string
* @version PHP 5
* @param $service_handle resource
* @param $action int
*/
function ibase_server_info($service_handle, $action);
/**
* Connect to the service manager
* @return resource
* @version PHP 5
* @param $host string
* @param $dba_username string
* @param $dba_password string
*/
function ibase_service_attach($host, $dba_username, $dba_password);
/**
* Disconnect from the service manager
* @return bool
* @version PHP 5
* @param $service_handle resource
*/
function ibase_service_detach($service_handle);
/**
* Register a callback function to be called when events are posted
* @return resource
* @version PHP 5
* @param $event_handler callback
* @param $event_name1 string
* @param $event_name2 (optional) string
* @param $params1 (optional) string
*/
function ibase_set_event_handler($event_handler, $event_name1, $event_name2, $params1);
/**
* Sets the format of timestamp, date and time type columns returned from queries
* @return int
* @version PHP 3 >= 3.0.6, PHP 4
* @param $format string
* @param $columntype (optional) int
*/
function ibase_timefmt($format, $columntype);
/**
* Begin a transaction
* @return resource
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $trans_args int
* @param $link_identifier (optional) resource
*/
function ibase_trans($trans_args, $link_identifier);
/**
* Wait for an event to be posted by the database
* @return string
* @version PHP 5
* @param $event_name1 string
* @param $event_name2 (optional) string
* @param $params1 (optional) string
*/
function ibase_wait_event($event_name1, $event_name2, $params1);
/**
* Create a new calendar
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $calendar string
*/
function icap_create_calendar($stream_id, $calendar);
/**
* Delete a calendar
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $calendar string
*/
function icap_delete_calendar($stream_id, $calendar);
/**
* Delete an event from an ICAP calendar
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $uid int
*/
function icap_delete_event($stream_id, $uid);
/**
* Fetches an event from the calendar stream/
* @return int
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $event_id int
* @param $options (optional) int
*/
function icap_fetch_event($stream_id, $event_id, $options);
/**
* Return a list of events that has an alarm triggered at the given datetime
* @return int
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $date array
* @param $time array
*/
function icap_list_alarms($stream_id, $date, $time);
/**
* Return a list of events between two given datetimes
* @return array
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $begin_date int
* @param $end_date (optional) int
*/
function icap_list_events($stream_id, $begin_date, $end_date);
/**
* Opens up an ICAP connection
* @return resource
* @version PHP 4 <= 4.2.3
* @param $calendar string
* @param $username string
* @param $password string
* @param $options string
*/
function icap_open($calendar, $username, $password, $options);
/**
* Rename a calendar
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $old_name string
* @param $new_name string
*/
function icap_rename_calendar($stream_id, $old_name, $new_name);
/**
* Reopen ICAP stream to new calendar
* @return int
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $calendar string
* @param $options (optional) int
*/
function icap_reopen($stream_id, $calendar, $options);
/**
* Snooze an alarm
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $uid int
*/
function icap_snooze($stream_id, $uid);
/**
* Store an event into an ICAP calendar
* @return string
* @version PHP 4 <= 4.2.3
* @param $stream_id int
* @param $event object
*/
function icap_store_event($stream_id, $event);
/**
* Convert string to requested character encoding
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $in_charset string
* @param $out_charset string
* @param $str string
*/
function iconv($in_charset, $out_charset, $str);
/**
* Retrieve internal configuration variables of iconv extension
* @return mixed
* @version PHP 4 >= 4.0.5, PHP 5
* @param $type string
*/
function iconv_get_encoding($type);
/**
* Decodes a MIME header field
* @return string
* @version PHP 5
* @param $encoded_header string
* @param $mode (optional) int
* @param $charset (optional) string
*/
function iconv_mime_decode($encoded_header, $mode, $charset);
/**
* Decodes multiple MIME header fields at once
* @return array
* @version PHP 5
* @param $encoded_headers string
* @param $mode (optional) int
* @param $charset (optional) string
*/
function iconv_mime_decode_headers($encoded_headers, $mode, $charset);
/**
* Composes a MIME header field
* @return string
* @version PHP 5
* @param $field_name string
* @param $field_value string
* @param $preferences (optional) array
*/
function iconv_mime_encode($field_name, $field_value, $preferences);
/**
* Set current setting for character encoding conversion
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $type string
* @param $charset string
*/
function iconv_set_encoding($type, $charset);
/**
* Returns the character count of string
* @return int
* @version PHP 5
* @param $str string
* @param $charset (optional) string
*/
function iconv_strlen($str, $charset);
/**
* Finds position of first occurrence of a needle within a haystack
* @return int
* @version PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
* @param $charset (optional) string
*/
function iconv_strpos($haystack, $needle, $offset, $charset);
/**
* Finds the last occurrence of a needle within a haystack
* @return int
* @version PHP 5
* @param $haystack string
* @param $needle string
* @param $charset (optional) string
*/
function iconv_strrpos($haystack, $needle, $charset);
/**
* Cut out part of a string
* @return string
* @version PHP 5
* @param $str string
* @param $offset int
* @param $length (optional) int
* @param $charset (optional) string
*/
function iconv_substr($str, $offset, $length, $charset);
/**
* Get the long name of an ID3v2 frame
* @return string
* @version PECL
* @param $frameId string
*/
function id3_get_frame_long_name($frameId);
/**
* Get the short name of an ID3v2 frame
* @return string
* @version PECL
* @param $frameId string
*/
function id3_get_frame_short_name($frameId);
/**
* Get the id for a genre
* @return int
* @version PECL
* @param $genre string
*/
function id3_get_genre_id($genre);
/**
* Get all possible genre values
* @return array
* @version PECL
*/
function id3_get_genre_list();
/**
* Get the name for a genre id
* @return string
* @version PECL
* @param $genre_id int
*/
function id3_get_genre_name($genre_id);
/**
* Get all information stored in an ID3 tag
* @return array
* @version PECL
* @param $filename string
* @param $version (optional) int
*/
function id3_get_tag($filename, $version);
/**
* Get version of an ID3 tag
* @return int
* @version PECL
* @param $filename string
*/
function id3_get_version($filename);
/**
* Remove an existing ID3 tag
* @return bool
* @version PECL
* @param $filename string
* @param $version (optional) int
*/
function id3_remove_tag($filename, $version);
/**
* Update information stored in an ID3 tag
* @return bool
* @version PECL
* @param $filename string
* @param $tag array
* @param $version (optional) int
*/
function id3_set_tag($filename, $tag, $version);
/**
* Format a local time/date as integer
* @return int
* @version PHP 5
* @param $format string
* @param $timestamp (optional) int
*/
function idate($format, $timestamp);
/**
* Deletes the slob object
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifxus_close_slob($bid);
/**
* Creates an slob object and opens it
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $mode int
*/
function ifxus_create_slob($mode);
/**
* Deletes the slob object
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifxus_free_slob($bid);
/**
* Opens an slob object
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
* @param $mode int
*/
function ifxus_open_slob($bid, $mode);
/**
* Reads nbytes of the slob object
* @return string
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
* @param $nbytes int
*/
function ifxus_read_slob($bid, $nbytes);
/**
* Sets the current file or seek position
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
* @param $mode int
* @param $offset int
*/
function ifxus_seek_slob($bid, $mode, $offset);
/**
* Returns the current file or seek position
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifxus_tell_slob($bid);
/**
* Writes a string into the slob object
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
* @param $content string
*/
function ifxus_write_slob($bid, $content);
/**
* Get number of rows affected by a query
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_affected_rows($result_id);
/**
* Set the default blob mode for all select queries
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $mode int
*/
function ifx_blobinfile_mode($mode);
/**
* Set the default byte mode
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $mode int
*/
function ifx_byteasvarchar($mode);
/**
* Close Informix connection
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function ifx_close($link_identifier);
/**
* Open Informix server connection
* @return resource
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $database string
* @param $userid (optional) string
* @param $password (optional) string
*/
function ifx_connect($database, $userid, $password);
/**
* Duplicates the given blob object
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifx_copy_blob($bid);
/**
* Creates an blob object
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $type int
* @param $mode int
* @param $param string
*/
function ifx_create_blob($type, $mode, $param);
/**
* Creates an char object
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $param string
*/
function ifx_create_char($param);
/**
* Execute a previously prepared SQL-statement
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_do($result_id);
/**
* Returns error code of last Informix call
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $connection_id resource
*/
function ifx_error($connection_id);
/**
* Returns error message of last Informix call
* @return string
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $errorcode int
*/
function ifx_errormsg($errorcode);
/**
* Get row as enumerated array
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
* @param $position (optional) mixed
*/
function ifx_fetch_row($result_id, $position);
/**
* List of SQL fieldproperties
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_fieldproperties($result_id);
/**
* List of Informix SQL fields
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_fieldtypes($result_id);
/**
* Deletes the blob object
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifx_free_blob($bid);
/**
* Deletes the char object
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $bid int
*/
function ifx_free_char($bid);
/**
* Releases resources for the query
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_free_result($result_id);
/**
* Get the contents of sqlca.sqlerrd[0..5] after a query
* @return array
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_getsqlca($result_id);
/**
* Return the content of a blob object
* @return string
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
*/
function ifx_get_blob($bid);
/**
* Return the content of the char object
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $bid int
*/
function ifx_get_char($bid);
/**
* Formats all rows of a query into a HTML table
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
* @param $html_table_options (optional) string
*/
function ifx_htmltbl_result($result_id, $html_table_options);
/**
* Sets the default return value on a fetch row
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $mode int
*/
function ifx_nullformat($mode);
/**
* Returns the number of columns in the query
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_num_fields($result_id);
/**
* Count the rows already fetched from a query
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result_id resource
*/
function ifx_num_rows($result_id);
/**
* Open persistent Informix connection
* @return resource
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $database string
* @param $userid (optional) string
* @param $password (optional) string
*/
function ifx_pconnect($database, $userid, $password);
/**
* Prepare an SQL-statement for execution
* @return resource
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $query string
* @param $conn_id resource
* @param $cursor_def (optional) int
* @param $blobidarray (optional) mixed
*/
function ifx_prepare($query, $conn_id, $cursor_def, $blobidarray);
/**
* Send Informix query
* @return resource
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $query string
* @param $link_identifier resource
* @param $cursor_type (optional) int
* @param $blobidarray (optional) mixed
*/
function ifx_query($query, $link_identifier, $cursor_type, $blobidarray);
/**
* Set the default text mode
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $mode int
*/
function ifx_textasvarchar($mode);
/**
* Updates the content of the blob object
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $bid int
* @param $content string
*/
function ifx_update_blob($bid, $content);
/**
* Updates the content of the char object
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $bid int
* @param $content string
*/
function ifx_update_char($bid, $content);
/**
* Set whether a client disconnect should abort script execution
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $setting bool
*/
function ignore_user_abort($setting);
/**
* Creates a new virtual web server
* @return int
* @version PECL
* @param $path string
* @param $comment string
* @param $server_ip string
* @param $port int
* @param $host_name string
* @param $rights int
* @param $start_server int
*/
function iis_add_server($path, $comment, $server_ip, $port, $host_name, $rights, $start_server);
/**
* Gets Directory Security
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
*/
function iis_get_dir_security($server_instance, $virtual_path);
/**
* Gets script mapping on a virtual directory for a specific extension
* @return string
* @version PECL
* @param $server_instance int
* @param $virtual_path string
* @param $script_extension string
*/
function iis_get_script_map($server_instance, $virtual_path, $script_extension);
/**
* Return the instance number associated with the Comment
* @return int
* @version PECL
* @param $comment string
*/
function iis_get_server_by_comment($comment);
/**
* Return the instance number associated with the Path
* @return int
* @version PECL
* @param $path string
*/
function iis_get_server_by_path($path);
/**
* Gets server rights
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
*/
function iis_get_server_rights($server_instance, $virtual_path);
/**
* Returns the state for the service defined by ServiceId
* @return int
* @version PECL
* @param $service_id string
*/
function iis_get_service_state($service_id);
/**
* Removes the virtual web server indicated by ServerInstance
* @return int
* @version PECL
* @param $server_instance int
*/
function iis_remove_server($server_instance);
/**
* Creates application scope for a virtual directory
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
* @param $application_scope string
*/
function iis_set_app_settings($server_instance, $virtual_path, $application_scope);
/**
* Sets Directory Security
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
* @param $directory_flags int
*/
function iis_set_dir_security($server_instance, $virtual_path, $directory_flags);
/**
* Sets script mapping on a virtual directory
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
* @param $script_extension string
* @param $engine_path string
* @param $allow_scripting int
*/
function iis_set_script_map($server_instance, $virtual_path, $script_extension, $engine_path, $allow_scripting);
/**
* Sets server rights
* @return int
* @version PECL
* @param $server_instance int
* @param $virtual_path string
* @param $directory_flags int
*/
function iis_set_server_rights($server_instance, $virtual_path, $directory_flags);
/**
* Starts the virtual web server
* @return int
* @version PECL
* @param $server_instance int
*/
function iis_start_server($server_instance);
/**
* Starts the service defined by ServiceId
* @return int
* @version PECL
* @param $service_id string
*/
function iis_start_service($service_id);
/**
* Stops the virtual web server
* @return int
* @version PECL
* @param $server_instance int
*/
function iis_stop_server($server_instance);
/**
* Stops the service defined by ServiceId
* @return int
* @version PECL
* @param $service_id string
*/
function iis_stop_service($service_id);
/**
* Output image to browser or file
* @return int
* @version PHP 4 >= 4.0.5, PHP 5
* @param $image resource
* @param $filename (optional) string
* @param $threshold (optional) int
*/
function image2wbmp($image, $filename, $threshold);
/**
* Set the blending mode for an image
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $blendmode bool
*/
function imagealphablending($image, $blendmode);
/**
* Should antialias functions be used or not
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $im resource
* @param $on bool
*/
function imageantialias($im, $on);
/**
* Draw a partial ellipse
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $cx int
* @param $cy int
* @param $w int
* @param $h int
* @param $s int
* @param $e int
* @param $color int
*/
function imagearc($image, $cx, $cy, $w, $h, $s, $e, $color);
/**
* Draw a character horizontally
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $font int
* @param $x int
* @param $y int
* @param $c string
* @param $color int
*/
function imagechar($image, $font, $x, $y, $c, $color);
/**
* Draw a character vertically
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $font int
* @param $x int
* @param $y int
* @param $c string
* @param $color int
*/
function imagecharup($image, $font, $x, $y, $c, $color);
/**
* Allocate a color for an image
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorallocate($image, $red, $green, $blue);
/**
* Allocate a color for an image
* @return int
* @version PHP 4 >= 4.3.2, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
* @param $alpha int
*/
function imagecolorallocatealpha($image, $red, $green, $blue, $alpha);
/**
* Get the index of the color of a pixel
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x int
* @param $y int
*/
function imagecolorat($image, $x, $y);
/**
* Get the index of the closest color to the specified color
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorclosest($image, $red, $green, $blue);
/**
* Get the index of the closest color to the specified color + alpha
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
* @param $alpha int
*/
function imagecolorclosestalpha($image, $red, $green, $blue, $alpha);
/**
* Get the index of the color which has the hue, white and blackness nearest to the given color
* @return int
* @version PHP 4 >= 4.0.1, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorclosesthwb($image, $red, $green, $blue);
/**
* De-allocate a color for an image
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $image resource
* @param $color int
*/
function imagecolordeallocate($image, $color);
/**
* Get the index of the specified color
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorexact($image, $red, $green, $blue);
/**
* Get the index of the specified color + alpha
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
* @param $alpha int
*/
function imagecolorexactalpha($image, $red, $green, $blue, $alpha);
/**
* Makes the colors of the palette version of an image more closely match the true color version
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $image1 resource
* @param $image2 resource
*/
function imagecolormatch($image1, $image2);
/**
* Get the index of the specified color or its closest possible alternative
* @return int
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorresolve($image, $red, $green, $blue);
/**
* Get the index of the specified color + alpha or its closest possible alternative
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $red int
* @param $green int
* @param $blue int
* @param $alpha int
*/
function imagecolorresolvealpha($image, $red, $green, $blue, $alpha);
/**
* Set the color for the specified palette index
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $index int
* @param $red int
* @param $green int
* @param $blue int
*/
function imagecolorset($image, $index, $red, $green, $blue);
/**
* Get the colors for an index
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $index int
*/
function imagecolorsforindex($image, $index);
/**
* Find out the number of colors in an image's palette
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
*/
function imagecolorstotal($image);
/**
* Define a color as transparent
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $color (optional) int
*/
function imagecolortransparent($image, $color);
/**
* Apply a 3x3 convolution matrix, using coefficient div and offset
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $image resource
* @param $matrix3x3 array
* @param $div float
* @param $offset float
*/
function imageconvolution($image, $matrix3x3, $div, $offset);
/**
* Copy part of an image
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $dst_im resource
* @param $src_im resource
* @param $dst_x int
* @param $dst_y int
* @param $src_x int
* @param $src_y int
* @param $src_w int
* @param $src_h int
*/
function imagecopy($dst_im, $src_im, $dst_x, $dst_y, $src_x, $src_y, $src_w, $src_h);
/**
* Copy and merge part of an image
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
* @param $dst_im resource
* @param $src_im resource
* @param $dst_x int
* @param $dst_y int
* @param $src_x int
* @param $src_y int
* @param $src_w int
* @param $src_h int
* @param $pct int
*/
function imagecopymerge($dst_im, $src_im, $dst_x, $dst_y, $src_x, $src_y, $src_w, $src_h, $pct);
/**
* Copy and merge part of an image with gray scale
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $dst_im resource
* @param $src_im resource
* @param $dst_x int
* @param $dst_y int
* @param $src_x int
* @param $src_y int
* @param $src_w int
* @param $src_h int
* @param $pct int
*/
function imagecopymergegray($dst_im, $src_im, $dst_x, $dst_y, $src_x, $src_y, $src_w, $src_h, $pct);
/**
* Copy and resize part of an image with resampling
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $dst_image resource
* @param $src_image resource
* @param $dst_x int
* @param $dst_y int
* @param $src_x int
* @param $src_y int
* @param $dst_w int
* @param $dst_h int
* @param $src_w int
* @param $src_h int
*/
function imagecopyresampled($dst_image, $src_image, $dst_x, $dst_y, $src_x, $src_y, $dst_w, $dst_h, $src_w, $src_h);
/**
* Copy and resize part of an image
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dst_image resource
* @param $src_image resource
* @param $dst_x int
* @param $dst_y int
* @param $src_x int
* @param $src_y int
* @param $dst_w int
* @param $dst_h int
* @param $src_w int
* @param $src_h int
*/
function imagecopyresized($dst_image, $src_image, $dst_x, $dst_y, $src_x, $src_y, $dst_w, $dst_h, $src_w, $src_h);
/**
* Create a new palette based image
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $x_size int
* @param $y_size int
*/
function imagecreate($x_size, $y_size);
/**
* Create a new image from GD file or URL
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $filename string
*/
function imagecreatefromgd($filename);
/**
* Create a new image from GD2 file or URL
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $filename string
*/
function imagecreatefromgd2($filename);
/**
* Create a new image from a given part of GD2 file or URL
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $filename string
* @param $srcX int
* @param $srcY int
* @param $width int
* @param $height int
*/
function imagecreatefromgd2part($filename, $srcX, $srcY, $width, $height);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function imagecreatefromgif($filename);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 3 >= 3.0.16, PHP 4, PHP 5
* @param $filename string
*/
function imagecreatefromjpeg($filename);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $filename string
*/
function imagecreatefrompng($filename);
/**
* Create a new image from the image stream in the string
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5
* @param $image string
*/
function imagecreatefromstring($image);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 4 >= 4.0.1, PHP 5
* @param $filename string
*/
function imagecreatefromwbmp($filename);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 4 >= 4.0.1, PHP 5
* @param $filename string
*/
function imagecreatefromxbm($filename);
/**
* Create a new image from file or URL
* @return resource
* @version PHP 4 >= 4.0.1, PHP 5
* @param $filename string
*/
function imagecreatefromxpm($filename);
/**
* Create a new true color image
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $x_size int
* @param $y_size int
*/
function imagecreatetruecolor($x_size, $y_size);
/**
* Draw a dashed line
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x1 int
* @param $y1 int
* @param $x2 int
* @param $y2 int
* @param $color int
*/
function imagedashedline($image, $x1, $y1, $x2, $y2, $color);
/**
* Destroy an image
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
*/
function imagedestroy($image);
/**
* Draw an ellipse
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $cx int
* @param $cy int
* @param $w int
* @param $h int
* @param $color int
*/
function imageellipse($image, $cx, $cy, $w, $h, $color);
/**
* Flood fill
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x int
* @param $y int
* @param $color int
*/
function imagefill($image, $x, $y, $color);
/**
* Draw a partial ellipse and fill it
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $cx int
* @param $cy int
* @param $w int
* @param $h int
* @param $s int
* @param $e int
* @param $color int
* @param $style int
*/
function imagefilledarc($image, $cx, $cy, $w, $h, $s, $e, $color, $style);
/**
* Draw a filled ellipse
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $cx int
* @param $cy int
* @param $w int
* @param $h int
* @param $color int
*/
function imagefilledellipse($image, $cx, $cy, $w, $h, $color);
/**
* Draw a filled polygon
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $points array
* @param $num_points int
* @param $color int
*/
function imagefilledpolygon($image, $points, $num_points, $color);
/**
* Draw a filled rectangle
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x1 int
* @param $y1 int
* @param $x2 int
* @param $y2 int
* @param $color int
*/
function imagefilledrectangle($image, $x1, $y1, $x2, $y2, $color);
/**
* Flood fill to specific color
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x int
* @param $y int
* @param $border int
* @param $color int
*/
function imagefilltoborder($image, $x, $y, $border, $color);
/**
* Applies a filter to an image
* @return bool
* @version PHP 5
* @param $src_im resource
* @param $filtertype int
* @param $arg1 (optional) int
* @param $arg2 (optional) int
* @param $arg3 (optional) int
*/
function imagefilter($src_im, $filtertype, $arg1, $arg2, $arg3);
/**
* Get font height
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $font int
*/
function imagefontheight($font);
/**
* Get font width
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $font int
*/
function imagefontwidth($font);
/**
* Give the bounding box of a text using fonts via freetype2
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $size float
* @param $angle float
* @param $font_file string
* @param $text string
* @param $extrainfo (optional) array
*/
function imageftbbox($size, $angle, $font_file, $text, $extrainfo);
/**
* Write text to the image using fonts using FreeType 2
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $image resource
* @param $size float
* @param $angle float
* @param $x int
* @param $y int
* @param $col int
* @param $font_file string
* @param $text string
* @param $extrainfo (optional) array
*/
function imagefttext($image, $size, $angle, $x, $y, $col, $font_file, $text, $extrainfo);
/**
* Apply a gamma correction to a GD image
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $image resource
* @param $inputgamma float
* @param $outputgamma float
*/
function imagegammacorrect($image, $inputgamma, $outputgamma);
/**
* Output GD image to browser or file
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $image resource
* @param $filename (optional) string
*/
function imagegd($image, $filename);
/**
* Output GD2 image to browser or file
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $image resource
* @param $filename (optional) string
* @param $chunk_size (optional) int
* @param $type (optional) int
*/
function imagegd2($image, $filename, $chunk_size, $type);
/**
* Output image to browser or file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $filename (optional) string
*/
function imagegif($image, $filename);
/**
* Enable or disable interlace
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $interlace (optional) int
*/
function imageinterlace($image, $interlace);
/**
* Finds whether an image is a truecolor image
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $image resource
*/
function imageistruecolor($image);
/**
* Output image to browser or file
* @return bool
* @version PHP 3 >= 3.0.16, PHP 4, PHP 5
* @param $image resource
* @param $filename (optional) string
* @param $quality (optional) int
*/
function imagejpeg($image, $filename, $quality);
/**
* Set the alpha blending flag to use the bundled libgd layering effects
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $image resource
* @param $effect int
*/
function imagelayereffect($image, $effect);
/**
* Draw a line
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x1 int
* @param $y1 int
* @param $x2 int
* @param $y2 int
* @param $color int
*/
function imageline($image, $x1, $y1, $x2, $y2, $color);
/**
* Load a new font
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $file string
*/
function imageloadfont($file);
/**
* Copy the palette from one image to another
* @return 
* @version PHP 4 >= 4.0.1, PHP 5
* @param $destination resource
* @param $source resource
*/
function imagepalettecopy($destination, $source);
/**
* Output a PNG image to either the browser or a file
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $image resource
* @param $filename (optional) string
*/
function imagepng($image, $filename);
/**
* Draw a polygon
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $points array
* @param $num_points int
* @param $color int
*/
function imagepolygon($image, $points, $num_points, $color);
/**
* Give the bounding box of a text rectangle using PostScript Type1 fonts
* @return array
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $text string
* @param $font int
* @param $size int
* @param $space (optional) int
* @param $tightness (optional) int
* @param $angle (optional) float
*/
function imagepsbbox($text, $font, $size, $space, $tightness, $angle);
/**
* Change the character encoding vector of a font
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $font_index resource
* @param $encodingfile string
*/
function imagepsencodefont($font_index, $encodingfile);
/**
* Extend or condense a font
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $font_index int
* @param $extend float
*/
function imagepsextendfont($font_index, $extend);
/**
* Free memory used by a PostScript Type 1 font
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $fontindex resource
*/
function imagepsfreefont($fontindex);
/**
* Load a PostScript Type 1 font from file
* @return resource
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $filename string
*/
function imagepsloadfont($filename);
/**
* Slant a font
* @return bool
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $font_index resource
* @param $slant float
*/
function imagepsslantfont($font_index, $slant);
/**
* To draw a text string over an image using PostScript Type1 fonts
* @return array
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $image resource
* @param $text string
* @param $font resource
* @param $size int
* @param $foreground int
* @param $background int
* @param $x int
* @param $y int
* @param $space (optional) int
* @param $tightness (optional) int
* @param $angle (optional) float
* @param $antialias_steps (optional) int
*/
function imagepstext($image, $text, $font, $size, $foreground, $background, $x, $y, $space, $tightness, $angle, $antialias_steps);
/**
* Draw a rectangle
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x1 int
* @param $y1 int
* @param $x2 int
* @param $y2 int
* @param $col int
*/
function imagerectangle($image, $x1, $y1, $x2, $y2, $col);
/**
* Rotate an image with a given angle
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $src_im resource
* @param $angle float
* @param $bgd_color int
* @param $ignore_transparent (optional) int
*/
function imagerotate($src_im, $angle, $bgd_color, $ignore_transparent);
/**
* Set the flag to save full alpha channel information (as opposed to single-color transparency) when saving PNG images
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $image resource
* @param $saveflag bool
*/
function imagesavealpha($image, $saveflag);
/**
* Set the brush image for line drawing
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $brush resource
*/
function imagesetbrush($image, $brush);
/**
* Set a single pixel
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $x int
* @param $y int
* @param $color int
*/
function imagesetpixel($image, $x, $y, $color);
/**
* Set the style for line drawing
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $style array
*/
function imagesetstyle($image, $style);
/**
* Set the thickness for line drawing
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $thickness int
*/
function imagesetthickness($image, $thickness);
/**
* Set the tile image for filling
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $tile resource
*/
function imagesettile($image, $tile);
/**
* Draw a string horizontally
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $font int
* @param $x int
* @param $y int
* @param $s string
* @param $col int
*/
function imagestring($image, $font, $x, $y, $s, $col);
/**
* Draw a string vertically
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $font int
* @param $x int
* @param $y int
* @param $s string
* @param $col int
*/
function imagestringup($image, $font, $x, $y, $s, $col);
/**
* Get image width
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
*/
function imagesx($image);
/**
* Get image height
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
*/
function imagesy($image);
/**
* Convert a true color image to a palette image
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $image resource
* @param $dither bool
* @param $ncolors int
*/
function imagetruecolortopalette($image, $dither, $ncolors);
/**
* Give the bounding box of a text using TrueType fonts
* @return array
* @version PHP 3 >= 3.0.1, PHP 4, PHP 5
* @param $size float
* @param $angle float
* @param $fontfile string
* @param $text string
*/
function imagettfbbox($size, $angle, $fontfile, $text);
/**
* Write text to the image using TrueType fonts
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $image resource
* @param $size float
* @param $angle float
* @param $x int
* @param $y int
* @param $color int
* @param $fontfile string
* @param $text string
*/
function imagettftext($image, $size, $angle, $x, $y, $color, $fontfile, $text);
/**
* Return the image types supported by this PHP build
* @return int
* @version PHP 3 CVS only, PHP 4 >= 4.0.2, PHP 5
*/
function imagetypes();
/**
* Output image to browser or file
* @return bool
* @version PHP 3 >= 3.0.15, PHP 4 >= 4.0.1, PHP 5
* @param $image resource
* @param $filename (optional) string
* @param $foreground (optional) int
*/
function imagewbmp($image, $filename, $foreground);
/**
* Output XBM image to browser or file
* @return bool
* @version PHP 5
* @param $image resource
* @param $filename string
* @param $foreground (optional) int
*/
function imagexbm($image, $filename, $foreground);
/**
* Get Mime-Type for image-type returned by getimagesize, exif_read_data, exif_thumbnail, exif_imagetype
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $imagetype int
*/
function image_type_to_mime_type($imagetype);
/**
* Convert an 8bit string to a quoted-printable string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function imap_8bit($string);
/**
* This function returns all IMAP alert messages (if any) that have occurred during this page request or since the alert stack was reset
* @return array
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
*/
function imap_alerts();
/**
* Append a string message to a specified mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mbox string
* @param $message string
* @param $options (optional) string
*/
function imap_append($imap_stream, $mbox, $message, $options);
/**
* Decode BASE64 encoded text
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $text string
*/
function imap_base64($text);
/**
* Convert an 8bit string to a base64 string
* @return string
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $string string
*/
function imap_binary($string);
/**
* Read the message body
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msg_number int
* @param $options (optional) int
*/
function imap_body($imap_stream, $msg_number, $options);
/**
* Read the structure of a specified body section of a specific message
* @return object
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $stream_id resource
* @param $msg_no int
* @param $section string
*/
function imap_bodystruct($stream_id, $msg_no, $section);
/**
* Check current mailbox
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_check($imap_stream);
/**
* Clears flags on messages
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $stream resource
* @param $sequence string
* @param $flag string
* @param $options (optional) string
*/
function imap_clearflag_full($stream, $sequence, $flag, $options);
/**
* Close an IMAP stream
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $flag (optional) int
*/
function imap_close($imap_stream, $flag);
/**
* Create a new mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mbox string
*/
function imap_createmailbox($imap_stream, $mbox);
/**
* Mark a message for deletion from current mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream int
* @param $msg_number int
* @param $options (optional) int
*/
function imap_delete($imap_stream, $msg_number, $options);
/**
* Delete a mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mbox string
*/
function imap_deletemailbox($imap_stream, $mbox);
/**
* This function returns all of the IMAP errors (if any) that have occurred during this page request or since the error stack was reset
* @return array
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
*/
function imap_errors();
/**
* Delete all messages marked for deletion
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_expunge($imap_stream);
/**
* Fetch a particular section of the body of the message
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msg_number int
* @param $part_number string
* @param $options (optional) int
*/
function imap_fetchbody($imap_stream, $msg_number, $part_number, $options);
/**
* Returns header for a message
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msgno int
* @param $options (optional) int
*/
function imap_fetchheader($imap_stream, $msgno, $options);
/**
* Read the structure of a particular message
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msg_number int
* @param $options (optional) int
*/
function imap_fetchstructure($imap_stream, $msg_number, $options);
/**
* Read an overview of the information in the headers of the given message
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $imap_stream resource
* @param $sequence string
* @param $options (optional) int
*/
function imap_fetch_overview($imap_stream, $sequence, $options);
/**
* Gets the ACL for a given mailbox
* @return array
* @version PHP 5
* @param $stream_id resource
* @param $mailbox string
*/
function imap_getacl($stream_id, $mailbox);
/**
* Read the list of mailboxes, returning detailed information on each one
* @return array
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $imap_stream resource
* @param $ref string
* @param $pattern string
*/
function imap_getmailboxes($imap_stream, $ref, $pattern);
/**
* List all the subscribed mailboxes
* @return array
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $imap_stream resource
* @param $ref string
* @param $pattern string
*/
function imap_getsubscribed($imap_stream, $ref, $pattern);
/**
* Retrieve the quota level settings, and usage statics per mailbox
* @return array
* @version PHP 4 >= 4.0.5, PHP 5
* @param $imap_stream resource
* @param $quota_root string
*/
function imap_get_quota($imap_stream, $quota_root);
/**
* Retrieve the quota settings per user
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $imap_stream resource
* @param $quota_root string
*/
function imap_get_quotaroot($imap_stream, $quota_root);
/**
* Alias of imap_headerinfo()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function imap_header();
/**
* Read the header of the message
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msg_number int
* @param $fromlength (optional) int
* @param $subjectlength (optional) int
* @param $defaulthost (optional) string
*/
function imap_headerinfo($imap_stream, $msg_number, $fromlength, $subjectlength, $defaulthost);
/**
* Returns headers for all messages in a mailbox
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_headers($imap_stream);
/**
* This function returns the last IMAP error (if any) that occurred during this page request
* @return string
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
*/
function imap_last_error();
/**
* Read the list of mailboxes
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $imap_stream resource
* @param $ref string
* @param $pattern string
*/
function imap_list($imap_stream, $ref, $pattern);
/**
* Alias of imap_list()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function imap_listmailbox();
/**
* Alias of imap_lsub()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function imap_listsubscribed();
/**
* List all the subscribed mailboxes
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $imap_stream resource
* @param $ref string
* @param $pattern string
*/
function imap_lsub($imap_stream, $ref, $pattern);
/**
* Send an email message
* @return bool
* @version PHP 3 >= 3.0.14, PHP 4, PHP 5
* @param $to string
* @param $subject string
* @param $message string
* @param $additional_headers (optional) string
* @param $cc (optional) string
* @param $bcc (optional) string
* @param $rpath (optional) string
*/
function imap_mail($to, $subject, $message, $additional_headers, $cc, $bcc, $rpath);
/**
* Get information about the current mailbox
* @return object
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_mailboxmsginfo($imap_stream);
/**
* Create a MIME message based on given envelope and body sections
* @return string
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
* @param $envelope array
* @param $body array
*/
function imap_mail_compose($envelope, $body);
/**
* Copy specified messages to a mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msglist string
* @param $mbox string
* @param $options (optional) int
*/
function imap_mail_copy($imap_stream, $msglist, $mbox, $options);
/**
* Move specified messages to a mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msglist string
* @param $mbox string
* @param $options (optional) int
*/
function imap_mail_move($imap_stream, $msglist, $mbox, $options);
/**
* Decode MIME header elements
* @return array
* @version PHP 3 >= 3.0.17, PHP 4, PHP 5
* @param $text string
*/
function imap_mime_header_decode($text);
/**
* This function returns the message sequence number for the given UID
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $uid int
*/
function imap_msgno($imap_stream, $uid);
/**
* Gives the number of messages in the current mailbox
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_num_msg($imap_stream);
/**
* Gives the number of recent messages in current mailbox
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_num_recent($imap_stream);
/**
* Open an IMAP stream to a mailbox
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $mailbox string
* @param $username string
* @param $password string
* @param $options (optional) int
*/
function imap_open($mailbox, $username, $password, $options);
/**
* Check if the IMAP stream is still active
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
*/
function imap_ping($imap_stream);
/**
* Convert a quoted-printable string to an 8 bit string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function imap_qprint($string);
/**
* Rename an old mailbox to new mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $old_mbox string
* @param $new_mbox string
*/
function imap_renamemailbox($imap_stream, $old_mbox, $new_mbox);
/**
* Reopen IMAP stream to new mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mailbox string
* @param $options (optional) int
*/
function imap_reopen($imap_stream, $mailbox, $options);
/**
* Parses an address string
* @return array
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $address string
* @param $default_host string
*/
function imap_rfc822_parse_adrlist($address, $default_host);
/**
* Parse mail headers from a string
* @return object
* @version PHP 4, PHP 5
* @param $headers string
* @param $defaulthost (optional) string
*/
function imap_rfc822_parse_headers($headers, $defaulthost);
/**
* Returns a properly formatted email address given the mailbox, host, and personal info
* @return string
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $mailbox string
* @param $host string
* @param $personal string
*/
function imap_rfc822_write_address($mailbox, $host, $personal);
/**
* Alias of imap_listscan()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function imap_scanmailbox();
/**
* This function returns an array of messages matching the given search criteria
* @return array
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $imap_stream resource
* @param $criteria string
* @param $options (optional) int
* @param $charset (optional) string
*/
function imap_search($imap_stream, $criteria, $options, $charset);
/**
* Sets the ACL for a giving mailbox
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $stream_id resource
* @param $mailbox string
* @param $id string
* @param $rights string
*/
function imap_setacl($stream_id, $mailbox, $id, $rights);
/**
* Sets flags on messages
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $stream resource
* @param $sequence string
* @param $flag string
* @param $options (optional) string
*/
function imap_setflag_full($stream, $sequence, $flag, $options);
/**
* Sets a quota for a given mailbox
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $imap_stream resource
* @param $quota_root string
* @param $quota_limit int
*/
function imap_set_quota($imap_stream, $quota_root, $quota_limit);
/**
* Sort an array of message headers
* @return array
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $stream resource
* @param $criteria int
* @param $reverse int
* @param $options (optional) int
* @param $search_criteria (optional) string
* @param $charset (optional) string
*/
function imap_sort($stream, $criteria, $reverse, $options, $search_criteria, $charset);
/**
* This function returns status information on a mailbox other than the current one
* @return object
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mailbox string
* @param $options int
*/
function imap_status($imap_stream, $mailbox, $options);
/**
* Subscribe to a mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $mbox string
*/
function imap_subscribe($imap_stream, $mbox);
/**
* Returns a tree of threaded message
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $stream_id resource
* @param $options (optional) int
*/
function imap_thread($stream_id, $options);
/**
* Set or fetch imap timeout
* @return mixed
* @version PHP 4 >= 4.3.3, PHP 5
* @param $timeout_type int
* @param $timeout (optional) int
*/
function imap_timeout($timeout_type, $timeout);
/**
* This function returns the UID for the given message sequence number
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msgno int
*/
function imap_uid($imap_stream, $msgno);
/**
* Unmark the message which is marked deleted
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream resource
* @param $msg_number int
* @param $flags (optional) int
*/
function imap_undelete($imap_stream, $msg_number, $flags);
/**
* Unsubscribe from a mailbox
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $imap_stream string
* @param $mbox string
*/
function imap_unsubscribe($imap_stream, $mbox);
/**
* Decodes a modified UTF-7 encoded string
* @return string
* @version PHP 3 >= 3.0.15, PHP 4, PHP 5
* @param $text string
*/
function imap_utf7_decode($text);
/**
* Converts ISO-8859-1 string to modified UTF-7 text
* @return string
* @version PHP 3 >= 3.0.15, PHP 4, PHP 5
* @param $data string
*/
function imap_utf7_encode($data);
/**
* Converts MIME-encoded text to UTF-8
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $mime_encoded_text string
*/
function imap_utf8($mime_encoded_text);
/**
* Join array elements with a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $glue string
* @param $pieces array
*/
function implode($glue, $pieces);
/**
* Import GET/POST/Cookie variables into the global scope
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $types string
* @param $prefix (optional) string
*/
function import_request_variables($types, $prefix);
/**
* N/A
* @return N/A
* @version N/A
*/
function include()();
/**
* N/A
* @return N/A
* @version N/A
*/
function include_once()();
/**
* Converts a packed internet address to a human readable representation
* @return string
* @version PHP 5 >= 5.1.0RC1
* @param $in_addr string
*/
function inet_ntop($in_addr);
/**
* Converts a human readable IP address to its packed in_addr representation
* @return string
* @version PHP 5 >= 5.1.0RC1
* @param $address string
*/
function inet_pton($address);
/**
* Switch autocommit on or off
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_autocommit($link);
/**
* Close an Ingres II database connection
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_close($link);
/**
* Commit a transaction
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_commit($link);
/**
* Open a connection to an Ingres II database
* @return resource
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $database string
* @param $username (optional) string
* @param $password (optional) string
*/
function ingres_connect($database, $username, $password);
/**
* Gets a cursor name for a given link resource
* @return string
* @version PECL
* @param $link resource
*/
function ingres_cursor($link);
/**
* Gets the last ingres error number generated
* @return int
* @version PECL
* @param $link resource
*/
function ingres_errno($link);
/**
* Gets a meaningful error message for the last error generated
* @return string
* @version PECL
* @param $link resource
*/
function ingres_error($link);
/**
* Gets the last SQLSTATE error code generated
* @return string
* @version PECL
* @param $link resource
*/
function ingres_errsqlstate($link);
/**
* Fetch a row of result into an array
* @return array
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $result_type int
* @param $link (optional) resource
*/
function ingres_fetch_array($result_type, $link);
/**
* Fetch a row of result into an object
* @return object
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $result_type int
* @param $link (optional) resource
*/
function ingres_fetch_object($result_type, $link);
/**
* Fetch a row of result into an enumerated array
* @return array
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_fetch_row($link);
/**
* Get the length of a field
* @return int
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_length($index, $link);
/**
* Get the name of a field in a query result
* @return string
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_name($index, $link);
/**
* Test if a field is nullable
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_nullable($index, $link);
/**
* Get the precision of a field
* @return int
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_precision($index, $link);
/**
* Get the scale of a field
* @return int
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_scale($index, $link);
/**
* Get the type of a field in a query result
* @return string
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $index int
* @param $link (optional) resource
*/
function ingres_field_type($index, $link);
/**
* Get the number of fields returned by the last query
* @return int
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_num_fields($link);
/**
* Get the number of rows affected or returned by the last query
* @return int
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_num_rows($link);
/**
* Open a persistent connection to an Ingres II database
* @return resource
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $database string
* @param $username (optional) string
* @param $password (optional) string
*/
function ingres_pconnect($database, $username, $password);
/**
* Send a SQL query to Ingres II
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $query string
* @param $link (optional) resource
*/
function ingres_query($query, $link);
/**
* Roll back a transaction
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5 <= 5.0.4
* @param $link resource
*/
function ingres_rollback($link);
/**
* Alias of ini_set()
* @return &#13;
* @version PHP 4, PHP 5
*/
function ini_alter();
/**
* Gets the value of a configuration option
* @return string
* @version PHP 4, PHP 5
* @param $varname string
*/
function ini_get($varname);
/**
* Gets all configuration options
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $extension string
*/
function ini_get_all($extension);
/**
* Restores the value of a configuration option
* @return 
* @version PHP 4, PHP 5
* @param $varname string
*/
function ini_restore($varname);
/**
* Sets the value of a configuration option
* @return string
* @version PHP 4, PHP 5
* @param $varname string
* @param $newvalue string
*/
function ini_set($varname, $newvalue);
/**
* Checks if the interface has been defined
* @return bool
* @version PHP 5 >= 5.0.2
* @param $interface_name string
* @param $autoload (optional) bool
*/
function interface_exists($interface_name, $autoload);
/**
* Get the integer value of a variable
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
* @param $base (optional) int
*/
function intval($var, $base);
/**
* Checks if a value exists in an array
* @return bool
* @version PHP 4, PHP 5
* @param $needle mixed
* @param $haystack array
* @param $strict (optional) bool
*/
function in_array($needle, $haystack, $strict);
/**
* Converts a string containing an (IPv4) Internet Protocol dotted address into a proper address
* @return int
* @version PHP 4, PHP 5
* @param $ip_address string
*/
function ip2long($ip_address);
/**
* Embed binary IPTC data into a JPEG image
* @return mixed
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $iptcdata string
* @param $jpeg_file_name string
* @param $spool (optional) int
*/
function iptcembed($iptcdata, $jpeg_file_name, $spool);
/**
* Parse a binary IPTC http://www.iptc.org/ block into single tags.
* @return array
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $iptcblock string
*/
function iptcparse($iptcblock);
/**
* Set channel mode flags for user
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
* @param $mode_spec string
* @param $nick string
*/
function ircg_channel_mode($connection, $channel, $mode_spec, $nick);
/**
* Close connection to server
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $connection resource
* @param $reason string
*/
function ircg_disconnect($connection, $reason);
/**
* Decodes a list of JS-encoded parameters
* @return array
* @version PHP 4 >= 4.3.0, PHP 5 <= 5.0.4
* @param $params string
*/
function ircg_eval_ecmascript_params($params);
/**
* Returns the error from previous IRCG operation
* @return array
* @version PHP 4 >= 4.1.0, PHP 5 <= 5.0.4
* @param $connection resource
*/
function ircg_fetch_error_msg($connection);
/**
* Get username for connection
* @return string
* @version PHP 4 >= 4.1.0, PHP 5 <= 5.0.4
* @param $connection resource
*/
function ircg_get_username($connection);
/**
* Encodes HTML preserving output
* @return string
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $html_string string
* @param $auto_links (optional) bool
* @param $conv_br (optional) bool
*/
function ircg_html_encode($html_string, $auto_links, $conv_br);
/**
* Add a user to your ignore list on a server
* @return 
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $nick string
*/
function ircg_ignore_add($connection, $nick);
/**
* Remove a user from your ignore list on a server
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $nick string
*/
function ircg_ignore_del($connection, $nick);
/**
* Invites nickname to channel
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
* @param $nickname string
*/
function ircg_invite($connection, $channel, $nickname);
/**
* Check connection status
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
*/
function ircg_is_conn_alive($connection);
/**
* Join a channel on a connected server
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
* @param $key (optional) string
*/
function ircg_join($connection, $channel, $key);
/**
* Kick a user out of a channel on server
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
* @param $nick string
* @param $reason string
*/
function ircg_kick($connection, $channel, $nick, $reason);
/**
* List topic/user count of channel(s)
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
*/
function ircg_list($connection, $channel);
/**
* Check for the existence of a format message set
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $name string
*/
function ircg_lookup_format_messages($name);
/**
* IRC network statistics
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection resource
*/
function ircg_lusers($connection);
/**
* Send message to channel or user on server
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $connection resource
* @param $recipient string
* @param $message string
* @param $suppress (optional) bool
*/
function ircg_msg($connection, $recipient, $message, $suppress);
/**
* Query visible usernames
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection int
* @param $channel string
* @param $target (optional) string
*/
function ircg_names($connection, $channel, $target);
/**
* Change nickname on server
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $nick string
*/
function ircg_nick($connection, $nick);
/**
* Encode special characters in nickname to be IRC-compliant
* @return string
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $nick string
*/
function ircg_nickname_escape($nick);
/**
* Decodes encoded nickname
* @return string
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $nick string
*/
function ircg_nickname_unescape($nick);
/**
* Send a notice to a user on server
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $recipient string
* @param $message string
*/
function ircg_notice($connection, $recipient, $message);
/**
* Elevates privileges to IRC OPER
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection resource
* @param $name string
* @param $password string
*/
function ircg_oper($connection, $name, $password);
/**
* Leave a channel on server
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
*/
function ircg_part($connection, $channel);
/**
* Connect to an IRC server
* @return resource
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $username string
* @param $server_ip (optional) string
* @param $server_port (optional) int
* @param $msg_format (optional) string
* @param $ctcp_messages (optional) array
* @param $user_settings (optional) array
* @param $bailout_on_trivial (optional) bool
*/
function ircg_pconnect($username, $server_ip, $server_port, $msg_format, $ctcp_messages, $user_settings, $bailout_on_trivial);
/**
* Register a format message set
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $name string
* @param $messages array
*/
function ircg_register_format_messages($name, $messages);
/**
* Set current connection for output
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5 <= 5.0.4
* @param $connection resource
*/
function ircg_set_current($connection);
/**
* Set logfile for connection
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $connection resource
* @param $path string
*/
function ircg_set_file($connection, $path);
/**
* Set action to be executed when connection dies
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $connection resource
* @param $host string
* @param $port int
* @param $data string
*/
function ircg_set_on_die($connection, $host, $port, $data);
/**
* Set topic for channel on server
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $channel string
* @param $new_topic string
*/
function ircg_topic($connection, $channel, $new_topic);
/**
* Queries server for WHO information
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $connection resource
* @param $mask string
* @param $ops_only (optional) bool
*/
function ircg_who($connection, $mask, $ops_only);
/**
* Query server for user information
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $connection resource
* @param $nick string
*/
function ircg_whois($connection, $nick);
/**
* Determine whether a variable is set
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
* @param $var (optional) mixed
* @param $params1 (optional) 
*/
function isset($var, $var, $params1);
/**
* Returns TRUE if the object is of this class or has this class as one of its parents
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $object object
* @param $class_name string
*/
function is_a($object, $class_name);
/**
* Finds whether a variable is an array
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function is_array($var);
/**
* Finds out whether a variable is a boolean
* @return bool
* @version PHP 4, PHP 5
* @param $var mixed
*/
function is_bool($var);
/**
* Verify that the contents of a variable can be called as a function
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $var mixed
* @param $syntax_only (optional) bool
* @param &$callable_name (optional) string
*/
function is_callable($var, $syntax_only, &$callable_name);
/**
* Tells whether the filename is a directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function is_dir($filename);
/**
* Alias of is_float()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function is_double();
/**
* Tells whether the filename is executable
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function is_executable($filename);
/**
* Tells whether the filename is a regular file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function is_file($filename);
/**
* Finds whether a value is a legal finite number
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $val float
*/
function is_finite($val);
/**
* Finds whether a variable is a float
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function is_float($var);
/**
* Finds whether a value is infinite
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $val float
*/
function is_infinite($val);
/**
* Find whether a variable is an integer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function is_int($var);
/**
* Alias of is_int()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function is_integer();
/**
* Tells whether the filename is a symbolic link
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function is_link($filename);
/**
* Alias of is_int()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function is_long();
/**
* Finds whether a value is not a number
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $val float
*/
function is_nan($val);
/**
* Finds whether a variable is NULL
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $var mixed
*/
function is_null($var);
/**
* Finds whether a variable is a number or a numeric string
* @return bool
* @version PHP 4, PHP 5
* @param $var mixed
*/
function is_numeric($var);
/**
* Finds whether a variable is an object
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function is_object($var);
/**
* Tells whether the filename is readable
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function is_readable($filename);
/**
* Alias of is_float()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function is_real();
/**
* Finds whether a variable is a resource
* @return bool
* @version PHP 4, PHP 5
* @param $var mixed
*/
function is_resource($var);
/**
* Finds whether a variable is a scalar
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $var mixed
*/
function is_scalar($var);
/**
* Checks if SOAP call was failed
* @return bool
* @version PHP 5
* @param $obj mixed
*/
function is_soap_fault($obj);
/**
* Finds whether a variable is a string
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function is_string($var);
/**
* Returns TRUE if the object has this class as one of its parents
* @return bool
* @version PHP 4, PHP 5
* @param $object mixed
* @param $class_name string
*/
function is_subclass_of($object, $class_name);
/**
* Tells whether the file was uploaded via HTTP POST
* @return bool
* @version PHP 3 >= 3.0.17, PHP 4 >= 4.0.3, PHP 5
* @param $filename string
*/
function is_uploaded_file($filename);
/**
* Tells whether the filename is writable
* @return bool
* @version PHP 4, PHP 5
* @param $filename string
*/
function is_writable($filename);
/**
* Alias of is_writable()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function is_writeable();
/**
* Count the elements in an iterator
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $iterator IteratorAggregate
*/
function iterator_count($iterator);
/**
* Copy the iterator into an array
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $iterator IteratorAggregate
*/
function iterator_to_array($iterator);
/**
* Clear last Java exception
* @return 
* @version PHP 4 >= 4.0.2, PECL
*/
function java_last_exception_clear();
/**
* Get last Java exception
* @return object
* @version PHP 4 >= 4.0.2, PECL
*/
function java_last_exception_get();
/**
* Returns the day of the week
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $julianday int
* @param $mode (optional) int
*/
function JDDayOfWeek($julianday, $mode);
/**
* Returns a month name
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $julianday int
* @param $mode int
*/
function JDMonthName($julianday, $mode);
/**
* Converts a Julian Day Count to the French Republican Calendar
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $juliandaycount int
*/
function JDToFrench($juliandaycount);
/**
* Converts Julian Day Count to Gregorian date
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $julianday int
*/
function JDToGregorian($julianday);
/**
* Converts a Julian day count to a Jewish calendar date
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $juliandaycount int
* @param $hebrew (optional) bool
* @param $fl (optional) int
*/
function jdtojewish($juliandaycount, $hebrew, $fl);
/**
* Converts a Julian Day Count to a Julian Calendar Date
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $julianday int
*/
function JDToJulian($julianday);
/**
* Convert Julian Day to Unix timestamp
* @return int
* @version PHP 4, PHP 5
* @param $jday int
*/
function jdtounix($jday);
/**
* Converts a date in the Jewish Calendar to Julian Day Count
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $month int
* @param $day int
* @param $year int
*/
function JewishToJD($month, $day, $year);
/**
* Alias of implode()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function join();
/**
* Convert JPEG image file to WBMP image file
* @return int
* @version PHP 4 >= 4.0.5, PHP 5
* @param $jpegname string
* @param $wbmpname string
* @param $d_height int
* @param $d_width int
* @param $threshold int
*/
function jpeg2wbmp($jpegname, $wbmpname, $d_height, $d_width, $threshold);
/**
* Converts a Julian Calendar date to Julian Day Count
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $month int
* @param $day int
* @param $year int
*/
function JulianToJD($month, $day, $year);
/**
* Changes the principal's password
* @return bool
* @version PECL
* @param $handle resource
* @param $principal string
* @param $password string
*/
function kadm5_chpass_principal($handle, $principal, $password);
/**
* Creates a kerberos principal with the given parameters
* @return bool
* @version PECL
* @param $handle resource
* @param $principal string
* @param $password (optional) string
* @param $options (optional) array
*/
function kadm5_create_principal($handle, $principal, $password, $options);
/**
* Deletes a kerberos principal
* @return bool
* @version PECL
* @param $handle resource
* @param $principal string
*/
function kadm5_delete_principal($handle, $principal);
/**
* Closes the connection to the admin server and releases all related resources
* @return bool
* @version PECL
* @param $handle resource
*/
function kadm5_destroy($handle);
/**
* Flush all changes to the Kerberos database, leaving the connection to the Kerberos admin server open
* @return bool
* @version PECL
* @param $handle resource
*/
function kadm5_flush($handle);
/**
* Gets all policies from the Kerberos database
* @return array
* @version PECL
* @param $handle resource
*/
function kadm5_get_policies($handle);
/**
* Gets the principal's entries from the Kerberos database
* @return array
* @version PECL
* @param $handle resource
* @param $principal string
*/
function kadm5_get_principal($handle, $principal);
/**
* Gets all principals from the Kerberos database
* @return array
* @version PECL
* @param $handle resource
*/
function kadm5_get_principals($handle);
/**
* Opens a connection to the KADM5 library and initializes any neccessary state information
* @return resource
* @version PECL
* @param $admin_server string
* @param $realm string
* @param $principal string
* @param $password string
*/
function kadm5_init_with_password($admin_server, $realm, $principal, $password);
/**
* Modifies a kerberos principal with the given parameters
* @return bool
* @version PECL
* @param $handle resource
* @param $principal string
* @param $options array
*/
function kadm5_modify_principal($handle, $principal, $options);
/**
* Fetch a key from an associative array
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function key(&$array);
/**
* Sort an array by key in reverse order
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function krsort(&$array, $sort_flags);
/**
* Sort an array by key
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function ksort(&$array, $sort_flags);
/**
* Combined linear congruential generator
* @return float
* @version PHP 4, PHP 5
*/
function lcg_value();
/**
* Translate 8859 characters to t61 characters
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $value string
*/
function ldap_8859_to_t61($value);
/**
* Add entries to LDAP directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $entry array
*/
function ldap_add($link_identifier, $dn, $entry);
/**
* Bind to LDAP directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $bind_rdn (optional) string
* @param $bind_password (optional) string
*/
function ldap_bind($link_identifier, $bind_rdn, $bind_password);
/**
* Alias of ldap_unbind()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function ldap_close();
/**
* Compare value of attribute found in entry specified with DN
* @return mixed
* @version PHP 4 >= 4.0.2, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $attribute string
* @param $value string
*/
function ldap_compare($link_identifier, $dn, $attribute, $value);
/**
* Connect to an LDAP server
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
* @param $port (optional) int
*/
function ldap_connect($hostname, $port);
/**
* Count the number of entries in a search
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_identifier resource
*/
function ldap_count_entries($link_identifier, $result_identifier);
/**
* Delete an entry from a directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
*/
function ldap_delete($link_identifier, $dn);
/**
* Convert DN to User Friendly Naming format
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $dn string
*/
function ldap_dn2ufn($dn);
/**
* Convert LDAP error number into string error message
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $errno int
*/
function ldap_err2str($errno);
/**
* Return the LDAP error number of the last LDAP command
* @return int
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $link_identifier resource
*/
function ldap_errno($link_identifier);
/**
* Return the LDAP error message of the last LDAP command
* @return string
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $link_identifier resource
*/
function ldap_error($link_identifier);
/**
* Splits DN into its component parts
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $dn string
* @param $with_attrib int
*/
function ldap_explode_dn($dn, $with_attrib);
/**
* Return first attribute
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
* @param &$ber_identifier int
*/
function ldap_first_attribute($link_identifier, $result_entry_identifier, &$ber_identifier);
/**
* Return first result id
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_identifier resource
*/
function ldap_first_entry($link_identifier, $result_identifier);
/**
* Return first reference
* @return resource
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link resource
* @param $result resource
*/
function ldap_first_reference($link, $result);
/**
* Free result memory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result_identifier resource
*/
function ldap_free_result($result_identifier);
/**
* Get attributes from a search result entry
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
*/
function ldap_get_attributes($link_identifier, $result_entry_identifier);
/**
* Get the DN of a result entry
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
*/
function ldap_get_dn($link_identifier, $result_entry_identifier);
/**
* Get all result entries
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_identifier resource
*/
function ldap_get_entries($link_identifier, $result_identifier);
/**
* Get the current value for given option
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $link_identifier resource
* @param $option int
* @param &$retval mixed
*/
function ldap_get_option($link_identifier, $option, &$retval);
/**
* Get all values from a result entry
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
* @param $attribute string
*/
function ldap_get_values($link_identifier, $result_entry_identifier, $attribute);
/**
* Get all binary values from a result entry
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
* @param $attribute string
*/
function ldap_get_values_len($link_identifier, $result_entry_identifier, $attribute);
/**
* Single-level search
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $base_dn string
* @param $filter string
* @param $attributes (optional) array
* @param $attrsonly (optional) int
* @param $sizelimit (optional) int
* @param $timelimit (optional) int
* @param $deref (optional) int
*/
function ldap_list($link_identifier, $base_dn, $filter, $attributes, $attrsonly, $sizelimit, $timelimit, $deref);
/**
* Modify an LDAP entry
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $entry array
*/
function ldap_modify($link_identifier, $dn, $entry);
/**
* Add attribute values to current attributes
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $entry array
*/
function ldap_mod_add($link_identifier, $dn, $entry);
/**
* Delete attribute values from current attributes
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $entry array
*/
function ldap_mod_del($link_identifier, $dn, $entry);
/**
* Replace attribute values with new ones
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $entry array
*/
function ldap_mod_replace($link_identifier, $dn, $entry);
/**
* Get the next attribute in result
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
* @param &$ber_identifier resource
*/
function ldap_next_attribute($link_identifier, $result_entry_identifier, &$ber_identifier);
/**
* Get next result entry
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $result_entry_identifier resource
*/
function ldap_next_entry($link_identifier, $result_entry_identifier);
/**
* Get next reference
* @return resource
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link resource
* @param $entry resource
*/
function ldap_next_reference($link, $entry);
/**
* Extract information from reference entry
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link resource
* @param $entry resource
* @param &$referrals array
*/
function ldap_parse_reference($link, $entry, &$referrals);
/**
* Extract information from result
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link resource
* @param $result resource
* @param &$errcode int
* @param &$matcheddn (optional) string
* @param &$errmsg (optional) string
* @param &$referrals (optional) array
*/
function ldap_parse_result($link, $result, &$errcode, &$matcheddn, &$errmsg, &$referrals);
/**
* Read an entry
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $base_dn string
* @param $filter string
* @param $attributes (optional) array
* @param $attrsonly (optional) int
* @param $sizelimit (optional) int
* @param $timelimit (optional) int
* @param $deref (optional) int
*/
function ldap_read($link_identifier, $base_dn, $filter, $attributes, $attrsonly, $sizelimit, $timelimit, $deref);
/**
* Modify the name of an entry
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link_identifier resource
* @param $dn string
* @param $newrdn string
* @param $newparent string
* @param $deleteoldrdn bool
*/
function ldap_rename($link_identifier, $dn, $newrdn, $newparent, $deleteoldrdn);
/**
* Bind to LDAP directory using SASL
* @return bool
* @version PHP 5
* @param $link resource
* @param $binddn (optional) string
* @param $password (optional) string
* @param $sasl_mech (optional) string
* @param $sasl_realm (optional) string
* @param $sasl_authz_id (optional) string
* @param $props (optional) string
*/
function ldap_sasl_bind($link, $binddn, $password, $sasl_mech, $sasl_realm, $sasl_authz_id, $props);
/**
* Search LDAP tree
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
* @param $base_dn string
* @param $filter string
* @param $attributes (optional) array
* @param $attrsonly (optional) int
* @param $sizelimit (optional) int
* @param $timelimit (optional) int
* @param $deref (optional) int
*/
function ldap_search($link_identifier, $base_dn, $filter, $attributes, $attrsonly, $sizelimit, $timelimit, $deref);
/**
* Set the value of the given option
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $link_identifier resource
* @param $option int
* @param $newval mixed
*/
function ldap_set_option($link_identifier, $option, $newval);
/**
* Set a callback function to do re-binds on referral chasing
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $link resource
* @param $callback callback
*/
function ldap_set_rebind_proc($link, $callback);
/**
* Sort LDAP result entries
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $link resource
* @param $result resource
* @param $sortfilter string
*/
function ldap_sort($link, $result, $sortfilter);
/**
* Start TLS
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $link resource
*/
function ldap_start_tls($link);
/**
* Translate t61 characters to 8859 characters
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $value string
*/
function ldap_t61_to_8859($value);
/**
* Unbind from LDAP directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function ldap_unbind($link_identifier);
/**
* Calculate Levenshtein distance between two strings
* @return int
* @version PHP 3 >= 3.0.17, PHP 4 >= 4.0.1, PHP 5
* @param $str1 string
* @param $str2 string
* @param $cost_ins (optional) int
* @param $cost_rep (optional) int
* @param $cost_del (optional) int
*/
function levenshtein($str1, $str2, $cost_ins, $cost_rep, $cost_del);
/**
* Clear libxml error buffer
* @return 
* @version PHP 5 >= 5.1.0RC1
*/
function libxml_clear_errors();
/**
* Retrieve array of errors
* @return array
* @version PHP 5 >= 5.1.0RC1
*/
function libxml_get_errors();
/**
* Retrieve last error from libxml
* @return LibXMLError
* @version PHP 5 >= 5.1.0RC1
*/
function libxml_get_last_error();
/**
* Set the streams context for the next libxml document load or write
* @return 
* @version PHP 5
* @param $streams_context resource
*/
function libxml_set_streams_context($streams_context);
/**
* Disable libxml errors and allow user to fetch error information as needed
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $use_errors bool
*/
function libxml_use_internal_errors($use_errors);
/**
* Create a hard link
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $target string
* @param $link string
*/
function link($target, $link);
/**
* Gets information about a link
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $path string
*/
function linkinfo($path);
/**
* Assign variables as if they were an array
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $varname mixed
* @param $params1 mixed
*/
function list($varname, $params1);
/**
* Get numeric formatting information
* @return array
* @version PHP 4 >= 4.0.5, PHP 5
*/
function localeconv();
/**
* Get the local time
* @return array
* @version PHP 4, PHP 5
* @param $timestamp int
* @param $is_associative (optional) bool
*/
function localtime($timestamp, $is_associative);
/**
* Natural logarithm
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
* @param $base (optional) float
*/
function log($arg, $base);
/**
* Base-10 logarithm
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function log10($arg);
/**
* Returns log(1 + number), computed in a way that is accurate even when the value of number is close to zero
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $number float
*/
function log1p($number);
/**
* Converts an (IPv4) Internet network address into a string in Internet standard dotted format
* @return string
* @version PHP 4, PHP 5
* @param $proper_address int
*/
function long2ip($proper_address);
/**
* Gives information about a file or symbolic link
* @return array
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $filename string
*/
function lstat($filename);
/**
* Strip whitespace (or other characters) from the beginning of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $charlist (optional) string
*/
function ltrim($str, $charlist);
/**
* LZF compression
* @return string
* @version PECL
* @param $data string
*/
function lzf_compress($data);
/**
* LZF decompression
* @return string
* @version PECL
* @param $data string
*/
function lzf_decompress($data);
/**
* Determines what LZF extension was optimized for
* @return int
* @version PECL
*/
function lzf_optimized_for();
/**
* Send mail
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $to string
* @param $subject string
* @param $message string
* @param $additional_headers (optional) string
* @param $additional_parameters (optional) string
*/
function mail($to, $subject, $message, $additional_headers, $additional_parameters);
/**
* Figures out the best way of encoding the content read from the file pointer fp, which must be seek-able
* @return string
* @version 4.1.0 - 4.1.2 only, PECL
* @param $fp resource
*/
function mailparse_determine_best_xfer_encoding($fp);
/**
* Returns a handle that can be used to parse a message
* @return resource
* @version 4.1.0 - 4.1.2 only, PECL
*/
function mailparse_msg_create();
/**
* Extracts/decodes a message section
* @return 
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045 resource
* @param $msgbody string
* @param $callbackfunc (optional) callback
*/
function mailparse_msg_extract_part($rfc2045, $msgbody, $callbackfunc);
/**
* Extracts/decodes a message section, decoding the transfer encoding
* @return string
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045 resource
* @param $filename string
* @param $callbackfunc (optional) callback
*/
function mailparse_msg_extract_part_file($rfc2045, $filename, $callbackfunc);
/**
* Frees a handle allocated by mailparse_msg_create()
* @return bool
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045buf resource
*/
function mailparse_msg_free($rfc2045buf);
/**
* Returns a handle on a given section in a mimemessage
* @return resource
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045 resource
* @param $mimesection string
*/
function mailparse_msg_get_part($rfc2045, $mimesection);
/**
* Returns an associative array of info about the message
* @return array
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045 resource
*/
function mailparse_msg_get_part_data($rfc2045);
/**
* Returns an array of mime section names in the supplied message
* @return array
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045 resource
*/
function mailparse_msg_get_structure($rfc2045);
/**
* Incrementally parse data into buffer
* @return bool
* @version 4.1.0 - 4.1.2 only, PECL
* @param $rfc2045buf resource
* @param $data string
*/
function mailparse_msg_parse($rfc2045buf, $data);
/**
* Parse file and return a resource representing the structure
* @return resource
* @version 4.1.0 - 4.1.2 only, PECL
* @param $filename string
*/
function mailparse_msg_parse_file($filename);
/**
* Parse addresses and returns a hash containing that data
* @return array
* @version 4.1.0 - 4.1.2 only, PECL
* @param $addresses string
*/
function mailparse_rfc822_parse_addresses($addresses);
/**
* Streams data from source file pointer, apply encoding and write to destfp
* @return bool
* @version 4.1.0 - 4.1.2 only, PECL
* @param $sourcefp resource
* @param $destfp resource
* @param $encoding string
*/
function mailparse_stream_encode($sourcefp, $destfp, $encoding);
/**
* Scans the data from fp and extract each embedded uuencoded file
* @return array
* @version PECL
* @param $fp resource
*/
function mailparse_uudecode_all($fp);
/**
* Find highest value
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $arg1 number
* @param $arg2 number
* @param $params1 (optional) number
*/
function max($arg1, $arg2, $params1);
/**
* 
* @return maxdb->affected_rows
* @version PECL
*/
function maxdb_affected_rows();
/**
* 
* @return maxdb->auto_commit
* @version PECL
*/
function maxdb_autocommit();
/**
* Alias of maxdb_stmt_bind_param()
* @return &#13;
* @version PECL
*/
function maxdb_bind_param();
/**
* Alias of maxdb_stmt_bind_result()
* @return &#13;
* @version PECL
*/
function maxdb_bind_result();
/**
* 
* @return maxdb->change_user
* @version PECL
*/
function maxdb_change_user();
/**
* 
* @return maxdb->character_set_name
* @version PECL
*/
function maxdb_character_set_name();
/**
* Alias of maxdb_character_set_name()
* @return &#13;
* @version PECL
*/
function maxdb_client_encoding();
/**
* 
* @return maxdb->close
* @version PECL
*/
function maxdb_close();
/**
* 
* @return maxdb->close_long_data
* @version PECL
*/
function maxdb_close_long_data();
/**
* 
* @return maxdb->commit
* @version PECL
*/
function maxdb_commit();
/**
* 
* @return maxdb
* @version PECL
* @param $Open --
*/
function maxdb_connect($Open);
/**
* Returns the error code from last connect call
* @return int
* @version PECL
*/
function maxdb_connect_errno();
/**
* Returns a string description of the last connect error
* @return string
* @version PECL
*/
function maxdb_connect_error();
/**
* 
* @return result->data_seek
* @version PECL
*/
function maxdb_data_seek();
/**
* Performs debugging operations
* @return 
* @version PECL
* @param $debug string
*/
function maxdb_debug($debug);
/**
* 
* @return maxdb->disable_reads_from_master
* @version PECL
*/
function maxdb_disable_reads_from_master();
/**
* Disable RPL parse
* @return bool
* @version PECL
* @param $link resource
*/
function maxdb_disable_rpl_parse($link);
/**
* Dump debugging information into the log
* @return bool
* @version PECL
* @param $link resource
*/
function maxdb_dump_debug_info($link);
/**
* Open a connection to an embedded MaxDB server
* @return resource
* @version PECL
* @param $dbname string
*/
function maxdb_embedded_connect($dbname);
/**
* Enable reads from master
* @return bool
* @version PECL
* @param $link resource
*/
function maxdb_enable_reads_from_master($link);
/**
* Enable RPL parse
* @return bool
* @version PECL
* @param $link resource
*/
function maxdb_enable_rpl_parse($link);
/**
* 
* @return maxdb->errno
* @version PECL
*/
function maxdb_errno();
/**
* Returns a string description of the last error
* @return Procedural
* @version PECL
*/
function maxdb_error();
/**
* Alias of maxdb_real_escape_string()
* @return &#13;
* @version PECL
*/
function maxdb_escape_string();
/**
* Alias of maxdb_stmt_execute()
* @return &#13;
* @version PECL
*/
function maxdb_execute();
/**
* Alias of maxdb_stmt_fetch()
* @return &#13;
* @version PECL
*/
function maxdb_fetch();
/**
* 
* @return result->fetch_array
* @version PECL
*/
function maxdb_fetch_array();
/**
* 
* @return maxdb->fetch_assoc
* @version PECL
*/
function maxdb_fetch_assoc();
/**
* 
* @return result->fetch_field
* @version PECL
*/
function maxdb_fetch_field();
/**
* 
* @return result->fetch_fields
* @version PECL
*/
function maxdb_fetch_fields();
/**
* 
* @return result->fetch_field_direct
* @version PECL
*/
function maxdb_fetch_field_direct();
/**
* 
* @return result->lengths
* @version PECL
*/
function maxdb_fetch_lengths();
/**
* 
* @return result->fetch_object
* @version PECL
*/
function maxdb_fetch_object();
/**
* 
* @return result->fetch_row
* @version PECL
*/
function maxdb_fetch_row();
/**
* 
* @return maxdb->field_count
* @version PECL
*/
function maxdb_field_count();
/**
* 
* @return result->field_seek
* @version PECL
*/
function maxdb_field_seek();
/**
* 
* @return result->current_field
* @version PECL
*/
function maxdb_field_tell();
/**
* 
* @return result->free
* @version PECL
*/
function maxdb_free_result();
/**
* Returns the MaxDB client version as a string
* @return string
* @version PECL
*/
function maxdb_get_client_info();
/**
* Get MaxDB client info
* @return int
* @version PECL
*/
function maxdb_get_client_version();
/**
* 
* @return maxdb->get_host_info
* @version PECL
*/
function maxdb_get_host_info();
/**
* Alias of maxdb_stmt_result_metadata()
* @return &#13;
* @version PECL
*/
function maxdb_get_metadata();
/**
* 
* @return maxdb->protocol_version
* @version PECL
*/
function maxdb_get_proto_info();
/**
* 
* @return maxdb->server_info
* @version PECL
*/
function maxdb_get_server_info();
/**
* Returns the version of the MaxDB server as an integer
* @return Procedural
* @version PECL
*/
function maxdb_get_server_version();
/**
* 
* @return maxdb->info
* @version PECL
*/
function maxdb_info();
/**
* Initializes MaxDB and returns an resource for use with maxdb_real_connect
* @return resource
* @version PECL
*/
function maxdb_init();
/**
* 
* @return maxdb->insert_id
* @version PECL
*/
function maxdb_insert_id();
/**
* 
* @return maxdb->kill
* @version PECL
*/
function maxdb_kill();
/**
* Enforce execution of a query on the master in a master/slave setup
* @return bool
* @version PECL
* @param $link resource
* @param $query string
*/
function maxdb_master_query($link, $query);
/**
* 
* @return maxdb->more_results
* @version PECL
*/
function maxdb_more_results();
/**
* 
* @return maxdb->multi_query
* @version PECL
*/
function maxdb_multi_query();
/**
* 
* @return maxdb->next_result
* @version PECL
*/
function maxdb_next_result();
/**
* 
* @return result->field_count
* @version PECL
*/
function maxdb_num_fields();
/**
* Gets the number of rows in a result
* @return Procedural
* @version PECL
*/
function maxdb_num_rows();
/**
* 
* @return maxdb->options
* @version PECL
*/
function maxdb_options();
/**
* Alias of maxdb_stmt_param_count()
* @return &#13;
* @version PECL
*/
function maxdb_param_count();
/**
* 
* @return maxdb->ping
* @version PECL
*/
function maxdb_ping();
/**
* 
* @return maxdb->prepare
* @version PECL
*/
function maxdb_prepare();
/**
* 
* @return maxdb->query
* @version PECL
*/
function maxdb_query();
/**
* 
* @return maxdb->real_connect
* @version PECL
*/
function maxdb_real_connect();
/**
* 
* @return maxdb->real_escape_string
* @version PECL
*/
function maxdb_real_escape_string();
/**
* 
* @return maxdb->real_query
* @version PECL
*/
function maxdb_real_query();
/**
* Enables or disables internal report functions
* @return bool
* @version PECL
* @param $flags int
*/
function maxdb_report($flags);
/**
* 
* @return maxdb->rollback
* @version PECL
*/
function maxdb_rollback();
/**
* Check if RPL parse is enabled
* @return int
* @version PECL
* @param $link resource
*/
function maxdb_rpl_parse_enabled($link);
/**
* RPL probe
* @return bool
* @version PECL
* @param $link resource
*/
function maxdb_rpl_probe($link);
/**
* 
* @return maxdb->rpl_query_type
* @version PECL
*/
function maxdb_rpl_query_type();
/**
* 
* @return maxdb->select_db
* @version PECL
*/
function maxdb_select_db();
/**
* Alias of maxdb_stmt_send_long_data()
* @return &#13;
* @version PECL
*/
function maxdb_send_long_data();
/**
* 
* @return maxdb->send_query
* @version PECL
*/
function maxdb_send_query();
/**
* Shut down the embedded server
* @return 
* @version PECL
*/
function maxdb_server_end();
/**
* Initialize embedded server
* @return bool
* @version PECL
* @param $server array
* @param $groups (optional) array
*/
function maxdb_server_init($server, $groups);
/**
* Alias of maxdb_options()
* @return &#13;
* @version PECL
*/
function maxdb_set_opt();
/**
* 
* @return maxdb->sqlstate
* @version PECL
*/
function maxdb_sqlstate();
/**
* 
* @return maxdb->ssl_set
* @version PECL
*/
function maxdb_ssl_set();
/**
* 
* @return maxdb->stat
* @version PECL
*/
function maxdb_stat();
/**
* 
* @return maxdb_stmt->affected_rows
* @version PECL
*/
function maxdb_stmt_affected_rows();
/**
* 
* @return stmt->bind_param
* @version PECL
*/
function maxdb_stmt_bind_param();
/**
* 
* @return stmt->bind_result
* @version PECL
*/
function maxdb_stmt_bind_result();
/**
* 
* @return maxdb_stmt->close
* @version PECL
*/
function maxdb_stmt_close();
/**
* 
* @return stmt->close_long_data
* @version PECL
*/
function maxdb_stmt_close_long_data();
/**
* 
* @return stmt->data_seek
* @version PECL
*/
function maxdb_stmt_data_seek();
/**
* 
* @return maxdb_stmt->errno
* @version PECL
*/
function maxdb_stmt_errno();
/**
* 
* @return maxdb_stmt->error
* @version PECL
*/
function maxdb_stmt_error();
/**
* 
* @return stmt->execute
* @version PECL
*/
function maxdb_stmt_execute();
/**
* 
* @return stmt->fetch
* @version PECL
*/
function maxdb_stmt_fetch();
/**
* 
* @return stmt->free_result
* @version PECL
*/
function maxdb_stmt_free_result();
/**
* 
* @return maxdb->stmt_init
* @version PECL
*/
function maxdb_stmt_init();
/**
* 
* @return stmt->num_rows
* @version PECL
*/
function maxdb_stmt_num_rows();
/**
* 
* @return stmt->param_count
* @version PECL
*/
function maxdb_stmt_param_count();
/**
* 
* @return stmt->prepare
* @version PECL
*/
function maxdb_stmt_prepare();
/**
* 
* @return stmt->reset
* @version PECL
*/
function maxdb_stmt_reset();
/**
* Returns result set metadata from a prepared statement
* @return Procedural
* @version PECL
*/
function maxdb_stmt_result_metadata();
/**
* 
* @return stmt->send_long_data
* @version PECL
*/
function maxdb_stmt_send_long_data();
/**
* Returns SQLSTATE error from previous statement operation
* @return string
* @version PECL
* @param $stmt resource
*/
function maxdb_stmt_sqlstate($stmt);
/**
* 
* @return maxdb->store_result
* @version PECL
*/
function maxdb_stmt_store_result();
/**
* 
* @return maxdb->store_result
* @version PECL
*/
function maxdb_store_result();
/**
* 
* @return maxdb->thread_id
* @version PECL
*/
function maxdb_thread_id();
/**
* 
* @return maxdb->use_result
* @version PECL
*/
function maxdb_use_result();
/**
* 
* @return maxdb->warning_count
* @version PECL
*/
function maxdb_warning_count();
/**
* Perform case folding on a string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $str string
* @param $mode int
* @param $encoding (optional) string
*/
function mb_convert_case($str, $mode, $encoding);
/**
* Convert character encoding
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $to_encoding string
* @param $from_encoding (optional) mixed
*/
function mb_convert_encoding($str, $to_encoding, $from_encoding);
/**
* Convert "kana" one from another ("zen-kaku", "han-kaku" and more)
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $option (optional) string
* @param $encoding (optional) string
*/
function mb_convert_kana($str, $option, $encoding);
/**
* Convert character code in variable(s)
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $to_encoding string
* @param $from_encoding mixed
* @param &$vars mixed
* @param &$... (optional) mixed
*/
function mb_convert_variables($to_encoding, $from_encoding, &$vars, &$...);
/**
* Decode string in MIME header field
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
*/
function mb_decode_mimeheader($str);
/**
* Decode HTML numeric string reference to character
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $convmap array
* @param $encoding (optional) string
*/
function mb_decode_numericentity($str, $convmap, $encoding);
/**
* Detect character encoding
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $encoding_list (optional) mixed
* @param $strict (optional) bool
*/
function mb_detect_encoding($str, $encoding_list, $strict);
/**
* Set/Get character encoding detection order
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $encoding_list mixed
*/
function mb_detect_order($encoding_list);
/**
* Encode string for MIME header
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $charset (optional) string
* @param $transfer_encoding (optional) string
* @param $linefeed (optional) string
*/
function mb_encode_mimeheader($str, $charset, $transfer_encoding, $linefeed);
/**
* Encode character to HTML numeric string reference
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $convmap array
* @param $encoding (optional) string
*/
function mb_encode_numericentity($str, $convmap, $encoding);
/**
* Regular expression match with multibyte support
* @return int
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $string string
* @param $regs (optional) array
*/
function mb_ereg($pattern, $string, $regs);
/**
* Regular expression match ignoring case with multibyte support
* @return int
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $string string
* @param $regs (optional) array
*/
function mb_eregi($pattern, $string, $regs);
/**
* Replace regular expression with multibyte support ignoring case
* @return string
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $replace string
* @param $string string
* @param $option (optional) string
*/
function mb_eregi_replace($pattern, $replace, $string, $option);
/**
* Regular expression match for multibyte string
* @return bool
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $string string
* @param $option (optional) string
*/
function mb_ereg_match($pattern, $string, $option);
/**
* Replace regular expression with multibyte support
* @return string
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $replacement string
* @param $string string
* @param $option (optional) string
*/
function mb_ereg_replace($pattern, $replacement, $string, $option);
/**
* Multibyte regular expression match for predefined multibyte string
* @return bool
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $option (optional) string
*/
function mb_ereg_search($pattern, $option);
/**
* Returns start point for next regular expression match
* @return int
* @version PHP 4 >= 4.2.0
*/
function mb_ereg_search_getpos();
/**
* Retrieve the result from the last multibyte regular expression match
* @return array
* @version PHP 4 >= 4.2.0
*/
function mb_ereg_search_getregs();
/**
* Setup string and regular expression for multibyte regular expression match
* @return bool
* @version PHP 4 >= 4.2.0
* @param $string string
* @param $pattern (optional) string
* @param $option (optional) string
*/
function mb_ereg_search_init($string, $pattern, $option);
/**
* Return position and length of matched part of multibyte regular expression for predefined multibyte string
* @return array
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $option (optional) string
*/
function mb_ereg_search_pos($pattern, $option);
/**
* Returns the matched part of multibyte regular expression
* @return array
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $option (optional) string
*/
function mb_ereg_search_regs($pattern, $option);
/**
* Set start point of next regular expression match
* @return bool
* @version PHP 4 >= 4.2.0
* @param $position int
*/
function mb_ereg_search_setpos($position);
/**
* Get internal settings of mbstring
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5
* @param $type string
*/
function mb_get_info($type);
/**
* Detect HTTP input character encoding
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $type string
*/
function mb_http_input($type);
/**
* Set/Get HTTP output character encoding
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $encoding string
*/
function mb_http_output($encoding);
/**
* Set/Get internal character encoding
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $encoding string
*/
function mb_internal_encoding($encoding);
/**
* Set/Get current language
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $language string
*/
function mb_language($language);
/**
* Returns an array of all supported encodings
* @return array
* @version PHP 5
*/
function mb_list_encodings();
/**
* Callback function converts character encoding in output buffer
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $contents string
* @param $status int
*/
function mb_output_handler($contents, $status);
/**
* Parse GET/POST/COOKIE data and set global variable
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $encoded_string string
* @param &$result (optional) array
*/
function mb_parse_str($encoded_string, &$result);
/**
* Get MIME charset string
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $encoding string
*/
function mb_preferred_mime_name($encoding);
/**
* Returns current encoding for multibyte regex as string
* @return mixed
* @version PHP 4 >= 4.2.0
* @param $encoding string
*/
function mb_regex_encoding($encoding);
/**
* Set/Get the default options for mbregex functions
* @return string
* @version PHP 4 >= 4.3.0
* @param $options string
*/
function mb_regex_set_options($options);
/**
* Send encoded mail
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $to string
* @param $subject string
* @param $message string
* @param $additional_headers (optional) string
* @param $additional_parameter (optional) string
*/
function mb_send_mail($to, $subject, $message, $additional_headers, $additional_parameter);
/**
* Split multibyte string using regular expression
* @return array
* @version PHP 4 >= 4.2.0
* @param $pattern string
* @param $string string
* @param $limit (optional) int
*/
function mb_split($pattern, $string, $limit);
/**
* Get part of string
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $start int
* @param $length (optional) int
* @param $encoding (optional) string
*/
function mb_strcut($str, $start, $length, $encoding);
/**
* Get truncated string with specified width
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $start int
* @param $width int
* @param $trimmarker (optional) string
* @param $encoding (optional) string
*/
function mb_strimwidth($str, $start, $width, $trimmarker, $encoding);
/**
* Get string length
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $encoding (optional) string
*/
function mb_strlen($str, $encoding);
/**
* Find position of first occurrence of string in a string
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
* @param $encoding (optional) string
*/
function mb_strpos($haystack, $needle, $offset, $encoding);
/**
* Find position of last occurrence of a string in a string
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $haystack string
* @param $needle string
* @param $encoding (optional) string
*/
function mb_strrpos($haystack, $needle, $encoding);
/**
* Make a string lowercase
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $str string
* @param $encoding (optional) string
*/
function mb_strtolower($str, $encoding);
/**
* Make a string uppercase
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $str string
* @param $encoding (optional) string
*/
function mb_strtoupper($str, $encoding);
/**
* Return width of string
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $encoding (optional) string
*/
function mb_strwidth($str, $encoding);
/**
* Set/Get substitution character
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $substrchar mixed
*/
function mb_substitute_character($substrchar);
/**
* Get part of string
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $str string
* @param $start int
* @param $length (optional) int
* @param $encoding (optional) string
*/
function mb_substr($str, $start, $length, $encoding);
/**
* Count the number of substring occurrences
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $haystack string
* @param $needle string
* @param $encoding (optional) string
*/
function mb_substr_count($haystack, $needle, $encoding);
/**
* Store a new event into an MCAL calendar
* @return int
* @version PHP 4, PECL
* @param $mcal_stream int
*/
function mcal_append_event($mcal_stream);
/**
* Close an MCAL stream
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $flags (optional) int
*/
function mcal_close($mcal_stream, $flags);
/**
* Create a new MCAL calendar
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $calendar string
*/
function mcal_create_calendar($stream, $calendar);
/**
* Compares two dates
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $a_year int
* @param $a_month int
* @param $a_day int
* @param $b_year int
* @param $b_month int
* @param $b_day int
*/
function mcal_date_compare($a_year, $a_month, $a_day, $b_year, $b_month, $b_day);
/**
* Returns TRUE if the given year, month, day is a valid date
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $year int
* @param $month int
* @param $day int
*/
function mcal_date_valid($year, $month, $day);
/**
* Returns the number of days in a month
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $month int
* @param $leap_year int
*/
function mcal_days_in_month($month, $leap_year);
/**
* Returns the day of the week of the given date
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $year int
* @param $month int
* @param $day int
*/
function mcal_day_of_week($year, $month, $day);
/**
* Returns the day of the year of the given date
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $year int
* @param $month int
* @param $day int
*/
function mcal_day_of_year($year, $month, $day);
/**
* Delete an MCAL calendar
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $calendar string
*/
function mcal_delete_calendar($stream, $calendar);
/**
* Delete an event from an MCAL calendar
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $event_id int
*/
function mcal_delete_event($mcal_stream, $event_id);
/**
* Adds an attribute and a value to the streams global event structure
* @return bool
* @version PHP 3 >= 3.0.15, PHP 4, PECL
* @param $stream int
* @param $attribute string
* @param $value string
*/
function mcal_event_add_attribute($stream, $attribute, $value);
/**
* Initializes a streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
*/
function mcal_event_init($stream);
/**
* Sets the alarm of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $alarm int
*/
function mcal_event_set_alarm($stream, $alarm);
/**
* Sets the category of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $category string
*/
function mcal_event_set_category($stream, $category);
/**
* Sets the class of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $class int
*/
function mcal_event_set_class($stream, $class);
/**
* Sets the description of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $description string
*/
function mcal_event_set_description($stream, $description);
/**
* Sets the end date and time of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $hour (optional) int
* @param $min (optional) int
* @param $sec (optional) int
*/
function mcal_event_set_end($stream, $year, $month, $day, $hour, $min, $sec);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $interval int
*/
function mcal_event_set_recur_daily($stream, $year, $month, $day, $interval);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $interval int
*/
function mcal_event_set_recur_monthly_mday($stream, $year, $month, $day, $interval);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $interval int
*/
function mcal_event_set_recur_monthly_wday($stream, $year, $month, $day, $interval);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.15, PHP 4, PECL
* @param $stream int
*/
function mcal_event_set_recur_none($stream);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $interval int
* @param $weekdays int
*/
function mcal_event_set_recur_weekly($stream, $year, $month, $day, $interval, $weekdays);
/**
* Sets the recurrence of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $interval int
*/
function mcal_event_set_recur_yearly($stream, $year, $month, $day, $interval);
/**
* Sets the start date and time of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $year int
* @param $month int
* @param $day int
* @param $hour (optional) int
* @param $min (optional) int
* @param $sec (optional) int
*/
function mcal_event_set_start($stream, $year, $month, $day, $hour, $min, $sec);
/**
* Sets the title of the streams global event structure
* @return 
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $title string
*/
function mcal_event_set_title($stream, $title);
/**
* Returns an object containing the current streams event structure
* @return object
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
*/
function mcal_fetch_current_stream_event($stream);
/**
* Fetches an event from the calendar stream
* @return object
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $event_id int
* @param $options (optional) int
*/
function mcal_fetch_event($mcal_stream, $event_id, $options);
/**
* Returns if the given year is a leap year or not
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $year int
*/
function mcal_is_leap_year($year);
/**
* Return a list of events that has an alarm triggered at the given datetime
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $begin_year (optional) int
* @param $begin_month (optional) int
* @param $begin_day (optional) int
* @param $end_year (optional) int
* @param $end_month (optional) int
* @param $end_day (optional) int
*/
function mcal_list_alarms($mcal_stream, $begin_year, $begin_month, $begin_day, $end_year, $end_month, $end_day);
/**
* Return a list of IDs for a date or a range of dates
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $begin_year (optional) int
* @param $begin_month (optional) int
* @param $begin_day (optional) int
* @param $end_year (optional) int
* @param $end_month (optional) int
* @param $end_day (optional) int
*/
function mcal_list_events($mcal_stream, $begin_year, $begin_month, $begin_day, $end_year, $end_month, $end_day);
/**
* Returns the next recurrence of the event
* @return object
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $weekstart int
* @param $next array
*/
function mcal_next_recurrence($stream, $weekstart, $next);
/**
* Opens up an MCAL connection
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $calendar string
* @param $username string
* @param $password string
* @param $options (optional) int
*/
function mcal_open($calendar, $username, $password, $options);
/**
* Opens up a persistent MCAL connection
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $calendar string
* @param $username string
* @param $password string
* @param $options (optional) int
*/
function mcal_popen($calendar, $username, $password, $options);
/**
* Rename an MCAL calendar
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream int
* @param $old_name string
* @param $new_name string
*/
function mcal_rename_calendar($stream, $old_name, $new_name);
/**
* Reopens an MCAL connection
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
* @param $calendar string
* @param $options (optional) int
*/
function mcal_reopen($mcal_stream, $calendar, $options);
/**
* Turn off an alarm for an event
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $stream_id int
* @param $event_id int
*/
function mcal_snooze($stream_id, $event_id);
/**
* Modify an existing event in an MCAL calendar
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $mcal_stream int
*/
function mcal_store_event($mcal_stream);
/**
* Returns TRUE if the given hour, minutes and seconds is a valid time
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PECL
* @param $hour int
* @param $minutes int
* @param $seconds int
*/
function mcal_time_valid($hour, $minutes, $seconds);
/**
* Returns the week number of the given date
* @return int
* @version PHP 4, PECL
* @param $day int
* @param $month int
* @param $year int
*/
function mcal_week_of_year($day, $month, $year);
/**
* Encrypt/decrypt data in CBC mode
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
* @param $key string
* @param $data string
* @param $mode int
* @param $iv (optional) string
*/
function mcrypt_cbc($cipher, $key, $data, $mode, $iv);
/**
* Encrypt/decrypt data in CFB mode
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
* @param $key string
* @param $data string
* @param $mode int
* @param $iv string
*/
function mcrypt_cfb($cipher, $key, $data, $mode, $iv);
/**
* Create an initialization vector (IV) from a random source
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $size int
* @param $source (optional) int
*/
function mcrypt_create_iv($size, $source);
/**
* Decrypts crypttext with given parameters
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $cipher string
* @param $key string
* @param $data string
* @param $mode string
* @param $iv (optional) string
*/
function mcrypt_decrypt($cipher, $key, $data, $mode, $iv);
/**
* Deprecated: Encrypt/decrypt data in ECB mode
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
* @param $key string
* @param $data string
* @param $mode int
*/
function mcrypt_ecb($cipher, $key, $data, $mode);
/**
* Encrypts plaintext with given parameters
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $cipher string
* @param $key string
* @param $data string
* @param $mode string
* @param $iv (optional) string
*/
function mcrypt_encrypt($cipher, $key, $data, $mode, $iv);
/**
* Returns the name of the opened algorithm
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_algorithms_name($td);
/**
* Returns the blocksize of the opened algorithm
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_block_size($td);
/**
* Returns the size of the IV of the opened algorithm
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_iv_size($td);
/**
* Returns the maximum supported keysize of the opened mode
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_key_size($td);
/**
* Returns the name of the opened mode
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_modes_name($td);
/**
* Returns an array with the supported keysizes of the opened algorithm
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_get_supported_key_sizes($td);
/**
* Checks whether the algorithm of the opened mode is a block algorithm
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_is_block_algorithm($td);
/**
* Checks whether the encryption of the opened mode works on blocks
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_is_block_algorithm_mode($td);
/**
* Checks whether the opened mode outputs blocks
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_is_block_mode($td);
/**
* This function runs a self test on the opened module
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_enc_self_test($td);
/**
* This function encrypts data
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
* @param $data string
*/
function mcrypt_generic($td, $data);
/**
* This function deinitializes an encryption module
* @return bool
* @version PHP 4 >= 4.1.1, PHP 5
* @param $td resource
*/
function mcrypt_generic_deinit($td);
/**
* This function terminates encryption
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_generic_end($td);
/**
* This function initializes all buffers needed for encryption
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
* @param $key string
* @param $iv string
*/
function mcrypt_generic_init($td, $key, $iv);
/**
* Get the block size of the specified cipher
* @return int
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
*/
function mcrypt_get_block_size($cipher);
/**
* Get the name of the specified cipher
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
*/
function mcrypt_get_cipher_name($cipher);
/**
* Returns the size of the IV belonging to a specific cipher/mode combination
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $cipher string
* @param $mode string
*/
function mcrypt_get_iv_size($cipher, $mode);
/**
* Get the key size of the specified cipher
* @return int
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
*/
function mcrypt_get_key_size($cipher);
/**
* Get an array of all supported ciphers
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $lib_dir string
*/
function mcrypt_list_algorithms($lib_dir);
/**
* Get an array of all supported modes
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $lib_dir string
*/
function mcrypt_list_modes($lib_dir);
/**
* Close the mcrypt module
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
*/
function mcrypt_module_close($td);
/**
* Returns the blocksize of the specified algorithm
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $lib_dir (optional) string
*/
function mcrypt_module_get_algo_block_size($algorithm, $lib_dir);
/**
* Returns the maximum supported keysize of the opened mode
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $lib_dir (optional) string
*/
function mcrypt_module_get_algo_key_size($algorithm, $lib_dir);
/**
* Returns an array with the supported keysizes of the opened algorithm
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $lib_dir (optional) string
*/
function mcrypt_module_get_supported_key_sizes($algorithm, $lib_dir);
/**
* This function checks whether the specified algorithm is a block algorithm
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $lib_dir (optional) string
*/
function mcrypt_module_is_block_algorithm($algorithm, $lib_dir);
/**
* Returns if the specified module is a block algorithm or not
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $mode string
* @param $lib_dir (optional) string
*/
function mcrypt_module_is_block_algorithm_mode($mode, $lib_dir);
/**
* Returns if the specified mode outputs blocks or not
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $mode string
* @param $lib_dir (optional) string
*/
function mcrypt_module_is_block_mode($mode, $lib_dir);
/**
* Opens the module of the algorithm and the mode to be used
* @return resource
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $algorithm_directory string
* @param $mode string
* @param $mode_directory string
*/
function mcrypt_module_open($algorithm, $algorithm_directory, $mode, $mode_directory);
/**
* This function runs a self test on the specified module
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $algorithm string
* @param $lib_dir (optional) string
*/
function mcrypt_module_self_test($algorithm, $lib_dir);
/**
* Encrypt/decrypt data in OFB mode
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $cipher int
* @param $key string
* @param $data string
* @param $mode int
* @param $iv string
*/
function mcrypt_ofb($cipher, $key, $data, $mode, $iv);
/**
* Calculate the md5 hash of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $raw_output (optional) bool
*/
function md5($str, $raw_output);
/**
* Calculates the md5 hash of a given file
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
* @param $raw_output (optional) bool
*/
function md5_file($filename, $raw_output);
/**
* Decrypt data
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $td resource
* @param $data string
*/
function mdecrypt_generic($td, $data);
/**
* Turn debug output on/off
* @return bool
* @version PECL
* @param $on_off bool
*/
function memcache_debug($on_off);
/**
* Returns the amount of memory allocated to PHP
* @return int
* @version PHP 4 >= 4.3.2, PHP 5
*/
function memory_get_usage();
/**
* Calculate the metaphone key of a string
* @return string
* @version PHP 4, PHP 5
* @param $str string
* @param $phones (optional) int
*/
function metaphone($str, $phones);
/**
* Checks if the class method exists
* @return bool
* @version PHP 4, PHP 5
* @param $object object
* @param $method_name string
*/
function method_exists($object, $method_name);
/**
* Compute hash
* @return string
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $hash int
* @param $data string
* @param $key (optional) string
*/
function mhash($hash, $data, $key);
/**
* Get the highest available hash id
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
*/
function mhash_count();
/**
* Get the block size of the specified hash
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $hash int
*/
function mhash_get_block_size($hash);
/**
* Get the name of the specified hash
* @return string
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $hash int
*/
function mhash_get_hash_name($hash);
/**
* Generates a key
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $hash int
* @param $password string
* @param $salt string
* @param $bytes int
*/
function mhash_keygen_s2k($hash, $password, $salt, $bytes);
/**
* Return current Unix timestamp with microseconds
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $get_as_float bool
*/
function microtime($get_as_float);
/**
* Detect MIME Content-type for a file
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $filename string
*/
function mime_content_type($filename);
/**
* Find lowest value
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $arg1 number
* @param $arg2 number
* @param $params1 (optional) number
*/
function min($arg1, $arg2, $params1);
/**
* Returns the action flag for keyPress(char)
* @return int
* @version PHP 5
* @param $str string
*/
function ming_keypress($str);
/**
* Set cubic threshold (?)
* @return 
* @version PHP 4 >= 4.0.5, PHP 5
* @param $threshold int
*/
function ming_setcubicthreshold($threshold);
/**
* Set scale (?)
* @return 
* @version PHP 4 >= 4.0.5, PHP 5
* @param $scale int
*/
function ming_setscale($scale);
/**
* Use constant pool (?)
* @return 
* @version PHP 5
* @param $use int
*/
function ming_useConstants($use);
/**
* Use SWF version (?)
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $version int
*/
function ming_useswfversion($version);
/**
* Makes directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $pathname string
* @param $mode (optional) int
* @param $recursive (optional) bool
* @param $context (optional) resource
*/
function mkdir($pathname, $mode, $recursive, $context);
/**
* Get Unix timestamp for a date
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $hour int
* @param $minute (optional) int
* @param $second (optional) int
* @param $month (optional) int
* @param $day (optional) int
* @param $year (optional) int
* @param $is_dst (optional) int
*/
function mktime($hour, $minute, $second, $month, $day, $year, $is_dst);
/**
* Formats a number as a currency string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $format string
* @param $number float
*/
function money_format($format, $number);
/**
* Moves an uploaded file to a new location
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $filename string
* @param $destination string
*/
function move_uploaded_file($filename, $destination);
/**
* Connect to msession server
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $host string
* @param $port string
*/
function msession_connect($host, $port);
/**
* Get session count
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
*/
function msession_count();
/**
* Create a session
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
*/
function msession_create($session);
/**
* Destroy a session
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $name string
*/
function msession_destroy($name);
/**
* Close connection to msession server
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
*/
function msession_disconnect();
/**
* Find all sessions with name and value
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $name string
* @param $value string
*/
function msession_find($name, $value);
/**
* Get value from session
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $name string
* @param $value string
*/
function msession_get($session, $name, $value);
/**
* Get array of msession variables
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
*/
function msession_get_array($session);
/**
* Get data session unstructured data
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
*/
function msession_get_data($session);
/**
* Increment value in session
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $name string
*/
function msession_inc($session, $name);
/**
* List all sessions
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
*/
function msession_list();
/**
* List sessions with variable
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $name string
*/
function msession_listvar($name);
/**
* Lock a session
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $name string
*/
function msession_lock($name);
/**
* Call an escape function within the msession personality plugin
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $val string
* @param $param (optional) string
*/
function msession_plugin($session, $val, $param);
/**
* Get random string
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $param int
*/
function msession_randstr($param);
/**
* Set value in session
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $name string
* @param $value string
*/
function msession_set($session, $name, $value);
/**
* Set msession variables from an array
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $tuples array
*/
function msession_set_array($session, $tuples);
/**
* Set data session unstructured data
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $value string
*/
function msession_set_data($session, $value);
/**
* Set/get session timeout
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $param (optional) int
*/
function msession_timeout($session, $param);
/**
* Get unique id
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $param int
*/
function msession_uniq($param);
/**
* Unlock a session
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $session string
* @param $key int
*/
function msession_unlock($session, $key);
/**
* Create or attach to a message queue
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $key int
* @param $perms (optional) int
*/
function msg_get_queue($key, $perms);
/**
* Receive a message from a message queue
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $queue resource
* @param $desiredmsgtype int
* @param &$msgtype int
* @param $maxsize int
* @param &$message mixed
* @param $unserialize (optional) bool
* @param $flags (optional) int
* @param &$errorcode (optional) int
*/
function msg_receive($queue, $desiredmsgtype, &$msgtype, $maxsize, &$message, $unserialize, $flags, &$errorcode);
/**
* Destroy a message queue
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $queue resource
*/
function msg_remove_queue($queue);
/**
* Send a message to a message queue
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $queue resource
* @param $msgtype int
* @param $message mixed
* @param $serialize (optional) bool
* @param $blocking (optional) bool
* @param &$errorcode (optional) int
*/
function msg_send($queue, $msgtype, $message, $serialize, $blocking, &$errorcode);
/**
* Set information in the message queue data structure
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $queue resource
* @param $data array
*/
function msg_set_queue($queue, $data);
/**
* Returns information from the message queue data structure
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $queue resource
*/
function msg_stat_queue($queue);
/**
* Alias of msql_db_query()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql();
/**
* Returns number of affected rows
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result resource
*/
function msql_affected_rows($result);
/**
* Close mSQL connection
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function msql_close($link_identifier);
/**
* Open mSQL connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
*/
function msql_connect($hostname);
/**
* Alias of msql_create_db()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_createdb();
/**
* Create mSQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function msql_create_db($database_name, $link_identifier);
/**
* Move internal row pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row_number int
*/
function msql_data_seek($result, $row_number);
/**
* Alias of msql_result()
* @return This
* @version PHP 3, PHP 4, PHP 5
*/
function msql_dbname();
/**
* Send mSQL query
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database string
* @param $query string
* @param $link_identifier (optional) resource
*/
function msql_db_query($database, $query, $link_identifier);
/**
* Drop (delete) mSQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function msql_drop_db($database_name, $link_identifier);
/**
* Returns error message of last msql call
* @return string
* @version PHP 3, PHP 4, PHP 5
*/
function msql_error();
/**
* Fetch row as array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $result_type (optional) int
*/
function msql_fetch_array($result, $result_type);
/**
* Get field information
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function msql_fetch_field($result, $field_offset);
/**
* Fetch row as object
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function msql_fetch_object($result);
/**
* Get row as enumerated array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function msql_fetch_row($result);
/**
* Alias of msql_field_flags()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_fieldflags();
/**
* Alias of msql_field_len()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_fieldlen();
/**
* Alias of msql_field_name()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_fieldname();
/**
* Alias of msql_field_table()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_fieldtable();
/**
* Alias of msql_field_type()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_fieldtype();
/**
* Get field flags
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_flags($result, $field_offset);
/**
* Get field length
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_len($result, $field_offset);
/**
* Get the name of the specified field in a result
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_name($result, $field_offset);
/**
* Set field offset
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_seek($result, $field_offset);
/**
* Get table name for field
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_table($result, $field_offset);
/**
* Get field type
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function msql_field_type($result, $field_offset);
/**
* Free result memory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function msql_free_result($result);
/**
* List mSQL databases on server
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function msql_list_dbs($link_identifier);
/**
* List result fields
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database string
* @param $tablename string
* @param $link_identifier (optional) resource
*/
function msql_list_fields($database, $tablename, $link_identifier);
/**
* List tables in an mSQL database
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database string
* @param $link_identifier (optional) resource
*/
function msql_list_tables($database, $link_identifier);
/**
* Alias of msql_num_fields()
* @return This
* @version PHP 3, PHP 4, PHP 5
*/
function msql_numfields();
/**
* Alias of msql_num_rows()
* @return This
* @version PHP 3, PHP 4, PHP 5
*/
function msql_numrows();
/**
* Get number of fields in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function msql_num_fields($result);
/**
* Get number of rows in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $query_identifier resource
*/
function msql_num_rows($query_identifier);
/**
* Open persistent mSQL connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
*/
function msql_pconnect($hostname);
/**
* Send mSQL query
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
*/
function msql_query($query, $link_identifier);
/**
* Alias of sql_regcase()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_regcase();
/**
* Get result data
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row int
* @param $field (optional) mixed
*/
function msql_result($result, $row, $field);
/**
* Select mSQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function msql_select_db($database_name, $link_identifier);
/**
* Alias of msql_result()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function msql_tablename();
/**
* Adds a parameter to a stored procedure or a remote stored procedure
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $stmt resource
* @param $param_name string
* @param &$var mixed
* @param $type int
* @param $is_output (optional) int
* @param $is_null (optional) int
* @param $maxlen (optional) int
*/
function mssql_bind($stmt, $param_name, &$var, $type, $is_output, $is_null, $maxlen);
/**
* Close MS SQL Server connection
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mssql_close($link_identifier);
/**
* Open MS SQL server connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $servername string
* @param $username (optional) string
* @param $password (optional) string
* @param $new_link (optional) bool
*/
function mssql_connect($servername, $username, $password, $new_link);
/**
* Moves internal row pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result_identifier resource
* @param $row_number int
*/
function mssql_data_seek($result_identifier, $row_number);
/**
* Executes a stored procedure on a MS SQL server database
* @return mixed
* @version PHP 4 >= 4.1.0, PHP 5
* @param $stmt resource
* @param $skip_results (optional) bool
*/
function mssql_execute($stmt, $skip_results);
/**
* Fetch a result row as an associative array, a numeric array, or both
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $result_type (optional) int
*/
function mssql_fetch_array($result, $result_type);
/**
* Returns an associative array of the current row in the result set specified by result_id
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result_id resource
*/
function mssql_fetch_assoc($result_id);
/**
* Returns the next batch of records
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $result_index resource
*/
function mssql_fetch_batch($result_index);
/**
* Get field information
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function mssql_fetch_field($result, $field_offset);
/**
* Fetch row as object
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mssql_fetch_object($result);
/**
* Get row as enumerated array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mssql_fetch_row($result);
/**
* Get the length of a field
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result resource
* @param $offset (optional) int
*/
function mssql_field_length($result, $offset);
/**
* Get the name of a field
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result resource
* @param $offset (optional) int
*/
function mssql_field_name($result, $offset);
/**
* Seeks to the specified field offset
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mssql_field_seek($result, $field_offset);
/**
* Gets the type of a field
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $result resource
* @param $offset (optional) int
*/
function mssql_field_type($result, $offset);
/**
* Free result memory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mssql_free_result($result);
/**
* Free statement memory
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $statement resource
*/
function mssql_free_statement($statement);
/**
* Returns the last message from the server
* @return string
* @version PHP 3, PHP 4, PHP 5
*/
function mssql_get_last_message();
/**
* Converts a 16 byte binary GUID to a string
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $binary string
* @param $short_format (optional) int
*/
function mssql_guid_string($binary, $short_format);
/**
* Initializes a stored procedure or a remote stored procedure
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $sp_name string
* @param $conn_id (optional) resource
*/
function mssql_init($sp_name, $conn_id);
/**
* Sets the lower error severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function mssql_min_error_severity($severity);
/**
* Sets the lower message severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function mssql_min_message_severity($severity);
/**
* Move the internal result pointer to the next result
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $result_id resource
*/
function mssql_next_result($result_id);
/**
* Gets the number of fields in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mssql_num_fields($result);
/**
* Gets the number of rows in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mssql_num_rows($result);
/**
* Open persistent MS SQL connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $servername string
* @param $username (optional) string
* @param $password (optional) string
* @param $new_link (optional) bool
*/
function mssql_pconnect($servername, $username, $password, $new_link);
/**
* Send MS SQL query
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
* @param $batch_size (optional) int
*/
function mssql_query($query, $link_identifier, $batch_size);
/**
* Get result data
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row int
* @param $field mixed
*/
function mssql_result($result, $row, $field);
/**
* Returns the number of records affected by the query
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $conn_id resource
*/
function mssql_rows_affected($conn_id);
/**
* Select MS SQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function mssql_select_db($database_name, $link_identifier);
/**
* Show largest possible random value
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
*/
function mt_getrandmax();
/**
* Generate a better random value
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $min int
* @param $max (optional) int
*/
function mt_rand($min, $max);
/**
* Seed the better random number generator
* @return 
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $seed int
*/
function mt_srand($seed);
/**
* Shuts down the muscat session and releases any memory back to PHP
* @return 
* @version 4.0.5 - 4.2.3 only, PECL
* @param $muscat_handle resource
*/
function muscat_close($muscat_handle);
/**
* Gets a line back from the core muscat API
* @return string
* @version 4.0.5 - 4.2.3 only, PECL
* @param $muscat_handle resource
*/
function muscat_get($muscat_handle);
/**
* Sends string to the core muscat API
* @return 
* @version 4.0.5 - 4.2.3 only, PECL
* @param $muscat_handle resource
* @param $string string
*/
function muscat_give($muscat_handle, $string);
/**
* Creates a new muscat session and returns the handle
* @return resource
* @version 4.0.5 - 4.2.3 only, PECL
* @param $size int
* @param $muscat_dir (optional) string
*/
function muscat_setup($size, $muscat_dir);
/**
* Creates a new muscat session and returns the handle
* @return resource
* @version 4.0.5 - 4.2.3 only, PECL
* @param $muscat_host string
*/
function muscat_setup_net($muscat_host);
/**
* 
* @return mysqli->affected_rows
* @version PHP 5
*/
function mysqli_affected_rows();
/**
* 
* @return mysqli->autocommit
* @version PHP 5
*/
function mysqli_autocommit();
/**
* Alias for mysqli_stmt_bind_param()
* @return &#13;
* @version PHP 5
*/
function mysqli_bind_param();
/**
* Alias for mysqli_stmt_bind_result()
* @return &#13;
* @version PHP 5
*/
function mysqli_bind_result();
/**
* 
* @return mysqli->change_user
* @version PHP 5
*/
function mysqli_change_user();
/**
* 
* @return mysqli->character_set_name
* @version PHP 5
*/
function mysqli_character_set_name();
/**
* Alias of mysqli_character_set_name()
* @return &#13;
* @version PHP 5
*/
function mysqli_client_encoding();
/**
* 
* @return mysqli->close
* @version PHP 5
*/
function mysqli_close();
/**
* 
* @return mysqli->commit
* @version PHP 5
*/
function mysqli_commit();
/**
* 
* @return mysqli
* @version PHP 5
* @param $Open --
*/
function mysqli_connect($Open);
/**
* Returns the error code from last connect call
* @return int
* @version PHP 5
*/
function mysqli_connect_errno();
/**
* Returns a string description of the last connect error
* @return string
* @version PHP 5
*/
function mysqli_connect_error();
/**
* 
* @return result->data_seek
* @version PHP 5
*/
function mysqli_data_seek();
/**
* Performs debugging operations
* @return bool
* @version PHP 5
* @param $debug string
*/
function mysqli_debug($debug);
/**
* 
* @return mysqli->disable_reads_from_master
* @version PHP 5
*/
function mysqli_disable_reads_from_master();
/**
* Disable RPL parse
* @return bool
* @version PHP 5
* @param $link mysqli
*/
function mysqli_disable_rpl_parse($link);
/**
* 
* @return mysqli->dump_debug_info
* @version PHP 5
*/
function mysqli_dump_debug_info();
/**
* Open a connection to an embedded mysql server
* @return mysqli
* @version PHP 5 <= 5.0.4
* @param $dbname string
*/
function mysqli_embedded_connect($dbname);
/**
* Enable reads from master
* @return bool
* @version PHP 5
* @param $link mysqli
*/
function mysqli_enable_reads_from_master($link);
/**
* Enable RPL parse
* @return bool
* @version PHP 5
* @param $link mysqli
*/
function mysqli_enable_rpl_parse($link);
/**
* 
* @return mysqli->errno
* @version PHP 5
*/
function mysqli_errno();
/**
* Returns a string description of the last error
* @return Procedural
* @version PHP 5
*/
function mysqli_error();
/**
* Alias of mysqli_real_escape_string()
* @return &#13;
* @version PHP 5
*/
function mysqli_escape_string();
/**
* Alias for mysqli_stmt_execute()
* @return &#13;
* @version PHP 5
*/
function mysqli_execute();
/**
* Alias for mysqli_stmt_fetch()
* @return &#13;
* @version PHP 5
*/
function mysqli_fetch();
/**
* 
* @return result->fetch_array
* @version PHP 5
*/
function mysqli_fetch_array();
/**
* 
* @return mysqli->fetch_assoc
* @version PHP 5
*/
function mysqli_fetch_assoc();
/**
* 
* @return result->fetch_field
* @version PHP 5
*/
function mysqli_fetch_field();
/**
* 
* @return result->fetch_fields
* @version PHP 5
*/
function mysqli_fetch_fields();
/**
* 
* @return result->fetch_field_direct
* @version PHP 5
*/
function mysqli_fetch_field_direct();
/**
* 
* @return result->lengths
* @version PHP 5
*/
function mysqli_fetch_lengths();
/**
* 
* @return result->fetch_object
* @version PHP 5
*/
function mysqli_fetch_object();
/**
* 
* @return result->fetch_row
* @version PHP 5
*/
function mysqli_fetch_row();
/**
* 
* @return mysqli->field_count
* @version PHP 5
*/
function mysqli_field_count();
/**
* 
* @return result->field_seek
* @version PHP 5
*/
function mysqli_field_seek();
/**
* 
* @return result->current_field
* @version PHP 5
*/
function mysqli_field_tell();
/**
* 
* @return result->free
* @version PHP 5
*/
function mysqli_free_result();
/**
* Returns the MySQL client version as a string
* @return string
* @version PHP 5
*/
function mysqli_get_client_info();
/**
* Get MySQL client info
* @return int
* @version PHP 5
*/
function mysqli_get_client_version();
/**
* 
* @return mysqli->get_host_info
* @version PHP 5
*/
function mysqli_get_host_info();
/**
* Alias for mysqli_stmt_result_metadata()
* @return &#13;
* @version PHP 5
*/
function mysqli_get_metadata();
/**
* 
* @return mysqli->protocol_version
* @version PHP 5
*/
function mysqli_get_proto_info();
/**
* 
* @return mysqli->server_info
* @version PHP 5
*/
function mysqli_get_server_info();
/**
* Returns the version of the MySQL server as an integer
* @return Procedural
* @version PHP 5
*/
function mysqli_get_server_version();
/**
* 
* @return mysqli->info
* @version PHP 5
*/
function mysqli_info();
/**
* Initializes MySQLi and returns a resource for use with mysqli_real_connect()
* @return mysqli
* @version PHP 5
*/
function mysqli_init();
/**
* 
* @return mysqli->insert_id
* @version PHP 5
*/
function mysqli_insert_id();
/**
* 
* @return mysqli->kill
* @version PHP 5
*/
function mysqli_kill();
/**
* Enforce execution of a query on the master in a master/slave setup
* @return bool
* @version PHP 5
* @param $link mysqli
* @param $query string
*/
function mysqli_master_query($link, $query);
/**
* 
* @return mysqli->more_results
* @version PHP 5
*/
function mysqli_more_results();
/**
* 
* @return mysqli->multi_query
* @version PHP 5
*/
function mysqli_multi_query();
/**
* 
* @return mysqli->next_result
* @version PHP 5
*/
function mysqli_next_result();
/**
* 
* @return result->field_count
* @version PHP 5
*/
function mysqli_num_fields();
/**
* Gets the number of rows in a result
* @return Procedural
* @version PHP 5
*/
function mysqli_num_rows();
/**
* 
* @return mysqli->options
* @version PHP 5
*/
function mysqli_options();
/**
* Alias for mysqli_stmt_param_count()
* @return &#13;
* @version PHP 5
*/
function mysqli_param_count();
/**
* 
* @return mysqli->ping
* @version PHP 5
*/
function mysqli_ping();
/**
* 
* @return mysqli->prepare
* @version PHP 5
*/
function mysqli_prepare();
/**
* 
* @return mysqli->query
* @version PHP 5
*/
function mysqli_query();
/**
* 
* @return mysqli->real_connect
* @version PHP 5
*/
function mysqli_real_connect();
/**
* 
* @return mysqli->real_escape_string
* @version PHP 5
*/
function mysqli_real_escape_string();
/**
* 
* @return mysqli->real_query
* @version PHP 5
*/
function mysqli_real_query();
/**
* Enables or disables internal report functions
* @return bool
* @version PHP 5
* @param $flags int
*/
function mysqli_report($flags);
/**
* 
* @return mysqli->rollback
* @version PHP 5
*/
function mysqli_rollback();
/**
* Check if RPL parse is enabled
* @return int
* @version PHP 5
* @param $link mysqli
*/
function mysqli_rpl_parse_enabled($link);
/**
* RPL probe
* @return bool
* @version PHP 5
* @param $link mysqli
*/
function mysqli_rpl_probe($link);
/**
* 
* @return mysqli->rpl_query_type
* @version PHP 5
*/
function mysqli_rpl_query_type();
/**
* 
* @return mysqli->select_db
* @version PHP 5
*/
function mysqli_select_db();
/**
* Alias for mysqli_stmt_send_long_data()
* @return &#13;
* @version PHP 5
*/
function mysqli_send_long_data();
/**
* 
* @return mysqli->send_query
* @version PHP 5
*/
function mysqli_send_query();
/**
* Shut down the embedded server
* @return 
* @version PHP 5 <= 5.0.4
*/
function mysqli_server_end();
/**
* Initialize embedded server
* @return bool
* @version PHP 5 <= 5.0.4
* @param $server array
* @param $groups (optional) array
*/
function mysqli_server_init($server, $groups);
/**
* 
* @return mysqli->set_charset
* @version PHP 5 >= 5.1.0RC1
*/
function mysqli_set_charset();
/**
* Alias of mysqli_options()
* @return &#13;
* @version PHP 5
*/
function mysqli_set_opt();
/**
* 
* @return mysqli->sqlstate
* @version PHP 5
*/
function mysqli_sqlstate();
/**
* 
* @return mysqli->ssl_set
* @version PHP 5
*/
function mysqli_ssl_set();
/**
* 
* @return mysqli->stat
* @version PHP 5
*/
function mysqli_stat();
/**
* 
* @return mysqli_stmt->affected_rows
* @version PHP 5
*/
function mysqli_stmt_affected_rows();
/**
* 
* @return stmt->bind_param
* @version PHP 5
*/
function mysqli_stmt_bind_param();
/**
* 
* @return stmt->bind_result
* @version PHP 5
*/
function mysqli_stmt_bind_result();
/**
* 
* @return mysqli_stmt->close
* @version PHP 5
*/
function mysqli_stmt_close();
/**
* 
* @return stmt->data_seek
* @version PHP 5
*/
function mysqli_stmt_data_seek();
/**
* 
* @return mysqli_stmt->errno
* @version PHP 5
*/
function mysqli_stmt_errno();
/**
* 
* @return mysqli_stmt->error
* @version PHP 5
*/
function mysqli_stmt_error();
/**
* 
* @return stmt->execute
* @version PHP 5
*/
function mysqli_stmt_execute();
/**
* 
* @return stmt->fetch
* @version PHP 5
*/
function mysqli_stmt_fetch();
/**
* 
* @return stmt->free_result
* @version PHP 5
*/
function mysqli_stmt_free_result();
/**
* 
* @return mysqli->stmt_init
* @version PHP 5
*/
function mysqli_stmt_init();
/**
* 
* @return stmt->num_rows
* @version PHP 5
*/
function mysqli_stmt_num_rows();
/**
* 
* @return stmt->param_count
* @version PHP 5
*/
function mysqli_stmt_param_count();
/**
* 
* @return stmt->prepare
* @version PHP 5
*/
function mysqli_stmt_prepare();
/**
* 
* @return stmt->reset
* @version PHP 5
*/
function mysqli_stmt_reset();
/**
* Returns result set metadata from a prepared statement
* @return Procedural
* @version PHP 5
*/
function mysqli_stmt_result_metadata();
/**
* 
* @return stmt->send_long_data
* @version PHP 5
*/
function mysqli_stmt_send_long_data();
/**
* Returns SQLSTATE error from previous statement operation
* @return string
* @version PHP 5
* @param $stmt mysqli_stmt
*/
function mysqli_stmt_sqlstate($stmt);
/**
* 
* @return mysqli_stmt->store_result
* @version PHP 5
*/
function mysqli_stmt_store_result();
/**
* 
* @return mysqli->store_result
* @version PHP 5
*/
function mysqli_store_result();
/**
* 
* @return mysqli->thread_id
* @version PHP 5
*/
function mysqli_thread_id();
/**
* Returns whether thread safety is given or not
* @return Procedural
* @version PHP 5
*/
function mysqli_thread_safe();
/**
* 
* @return mysqli->use_result
* @version PHP 5
*/
function mysqli_use_result();
/**
* 
* @return mysqli->warning_count
* @version PHP 5
*/
function mysqli_warning_count();
/**
* Get number of affected rows in previous MySQL operation
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_affected_rows($link_identifier);
/**
* Change logged in user of the active connection
* @return int
* @version PHP 3 >= 3.0.13
* @param $user string
* @param $password string
* @param $database (optional) string
* @param $link_identifier (optional) resource
*/
function mysql_change_user($user, $password, $database, $link_identifier);
/**
* Returns the name of the character set
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_client_encoding($link_identifier);
/**
* Close MySQL connection
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_close($link_identifier);
/**
* Open a connection to a MySQL Server
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $server string
* @param $username (optional) string
* @param $password (optional) string
* @param $new_link (optional) bool
* @param $client_flags (optional) int
*/
function mysql_connect($server, $username, $password, $new_link, $client_flags);
/**
* Create a MySQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function mysql_create_db($database_name, $link_identifier);
/**
* Move internal result pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row_number int
*/
function mysql_data_seek($result, $row_number);
/**
* Get result data
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result resource
* @param $row int
* @param $field (optional) mixed
*/
function mysql_db_name($result, $row, $field);
/**
* Send a MySQL query
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database string
* @param $query string
* @param $link_identifier (optional) resource
*/
function mysql_db_query($database, $query, $link_identifier);
/**
* Drop (delete) a MySQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function mysql_drop_db($database_name, $link_identifier);
/**
* Returns the numerical value of the error message from previous MySQL operation
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_errno($link_identifier);
/**
* Returns the text of the error message from previous MySQL operation
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_error($link_identifier);
/**
* Escapes a string for use in a mysql_query
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $unescaped_string string
*/
function mysql_escape_string($unescaped_string);
/**
* Fetch a result row as an associative array, a numeric array, or both
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $result_type (optional) int
*/
function mysql_fetch_array($result, $result_type);
/**
* Fetch a result row as an associative array
* @return array
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result resource
*/
function mysql_fetch_assoc($result);
/**
* Get column information from a result and return as an object
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function mysql_fetch_field($result, $field_offset);
/**
* Get the length of each output in a result
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_fetch_lengths($result);
/**
* Fetch a result row as an object
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_fetch_object($result);
/**
* Get a result row as an enumerated array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_fetch_row($result);
/**
* Get the flags associated with the specified field in a result
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_flags($result, $field_offset);
/**
* Returns the length of the specified field
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_len($result, $field_offset);
/**
* Get the name of the specified field in a result
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_name($result, $field_offset);
/**
* Set result pointer to a specified field offset
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_seek($result, $field_offset);
/**
* Get name of the table the specified field is in
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_table($result, $field_offset);
/**
* Get the type of the specified field in a result
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function mysql_field_type($result, $field_offset);
/**
* Free result memory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_free_result($result);
/**
* Get MySQL client info
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
*/
function mysql_get_client_info();
/**
* Get MySQL host info
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link_identifier resource
*/
function mysql_get_host_info($link_identifier);
/**
* Get MySQL protocol info
* @return int
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link_identifier resource
*/
function mysql_get_proto_info($link_identifier);
/**
* Get MySQL server info
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $link_identifier resource
*/
function mysql_get_server_info($link_identifier);
/**
* Get information about the most recent query
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_info($link_identifier);
/**
* Get the ID generated from the previous INSERT operation
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_insert_id($link_identifier);
/**
* List databases available on a MySQL server
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function mysql_list_dbs($link_identifier);
/**
* List MySQL table fields
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $table_name string
* @param $link_identifier (optional) resource
*/
function mysql_list_fields($database_name, $table_name, $link_identifier);
/**
* List MySQL processes
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_list_processes($link_identifier);
/**
* List tables in a MySQL database
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $database string
* @param $link_identifier (optional) resource
*/
function mysql_list_tables($database, $link_identifier);
/**
* Get number of fields in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_num_fields($result);
/**
* Get number of rows in result
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function mysql_num_rows($result);
/**
* Open a persistent connection to a MySQL server
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $server string
* @param $username (optional) string
* @param $password (optional) string
* @param $client_flags (optional) int
*/
function mysql_pconnect($server, $username, $password, $client_flags);
/**
* Ping a server connection or reconnect if there is no connection
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_ping($link_identifier);
/**
* Send a MySQL query
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
*/
function mysql_query($query, $link_identifier);
/**
* Escapes special characters in a string for use in a SQL statement
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $unescaped_string string
* @param $link_identifier (optional) resource
*/
function mysql_real_escape_string($unescaped_string, $link_identifier);
/**
* Get result data
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row int
* @param $field (optional) mixed
*/
function mysql_result($result, $row, $field);
/**
* Select a MySQL database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function mysql_select_db($database_name, $link_identifier);
/**
* Get current system status
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_stat($link_identifier);
/**
* Get table name of field
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $i int
*/
function mysql_tablename($result, $i);
/**
* Return the current thread ID
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $link_identifier resource
*/
function mysql_thread_id($link_identifier);
/**
* Send an SQL query to MySQL, without fetching and buffering the result rows
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
*/
function mysql_unbuffered_query($query, $link_identifier);
/**
* Check to see if a transaction has completed
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_checkstatus($conn, $identifier);
/**
* Number of complete authorizations in queue, returning an array of their identifiers
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param &$array int
*/
function m_completeauthorizations($conn, &$array);
/**
* Establish the connection to MCVE
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_connect($conn);
/**
* Get a textual representation of why a connection failed
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_connectionerror($conn);
/**
* Delete specified transaction from MCVE_CONN structure
* @return bool
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_deletetrans($conn, $identifier);
/**
* Destroy the connection and MCVE_CONN structure
* @return bool
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_destroyconn($conn);
/**
* Free memory associated with IP/SSL connectivity
* @return 
* @version PHP 4 >= 4.3.9, PHP 5
*/
function m_destroyengine();
/**
* Get a specific cell from a comma delimited response by column name
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
* @param $column string
* @param $row int
*/
function m_getcell($conn, $identifier, $column, $row);
/**
* Get a specific cell from a comma delimited response by column number
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
* @param $column int
* @param $row int
*/
function m_getcellbynum($conn, $identifier, $column, $row);
/**
* Get the RAW comma delimited data returned from MCVE
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_getcommadelimited($conn, $identifier);
/**
* Get the name of the column in a comma-delimited response
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
* @param $column_num int
*/
function m_getheader($conn, $identifier, $column_num);
/**
* Create and initialize an MCVE_CONN structure
* @return resource
* @version PHP 4 >= 4.3.9, PHP 5
*/
function m_initconn();
/**
* Ready the client for IP/SSL Communication
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $location string
*/
function m_initengine($location);
/**
* Checks to see if response is comma delimited
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_iscommadelimited($conn, $identifier);
/**
* The maximum amount of time the API will attempt a connection to MCVE
* @return bool
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $secs int
*/
function m_maxconntimeout($conn, $secs);
/**
* Perform communication with MCVE (send/receive data) Non-blocking
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_monitor($conn);
/**
* Number of columns returned in a comma delimited response
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_numcolumns($conn, $identifier);
/**
* Number of rows returned in a comma delimited response
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_numrows($conn, $identifier);
/**
* Parse the comma delimited response so m_getcell, etc will work
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_parsecommadelimited($conn, $identifier);
/**
* Returns array of strings which represents the keys that can be used for response parameters on this transaction
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $conn resource
* @param $identifier int
*/
function m_responsekeys($conn, $identifier);
/**
* Get a custom response parameter
* @return string
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
* @param $key string
*/
function m_responseparam($conn, $identifier, $key);
/**
* Check to see if the transaction was successful
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_returnstatus($conn, $identifier);
/**
* Set blocking/non-blocking mode for connection
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $tf int
*/
function m_setblocking($conn, $tf);
/**
* Set the connection method to Drop-File
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $directory string
*/
function m_setdropfile($conn, $directory);
/**
* Set the connection method to IP
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $host string
* @param $port int
*/
function m_setip($conn, $host, $port);
/**
* Set the connection method to SSL
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $host string
* @param $port int
*/
function m_setssl($conn, $host, $port);
/**
* Set SSL CA (Certificate Authority) file for verification of server certificate
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $conn resource
* @param $cafile string
*/
function m_setssl_cafile($conn, $cafile);
/**
* Set certificate key files and certificates if server requires client certificate verification
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $sslkeyfile string
* @param $sslcertfile string
*/
function m_setssl_files($conn, $sslkeyfile, $sslcertfile);
/**
* Set maximum transaction time (per trans)
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $seconds int
*/
function m_settimeout($conn, $seconds);
/**
* Check to see if outgoing buffer is clear
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_transactionssent($conn);
/**
* Number of transactions in client-queue
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_transinqueue($conn);
/**
* Add key/value pair to a transaction. Replaces deprecated transparam()
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $conn resource
* @param $identifier int
* @param $key string
* @param $value string
*/
function m_transkeyval($conn, $identifier, $key, $value);
/**
* Start a new transaction
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
*/
function m_transnew($conn);
/**
* Finalize and send the transaction
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $identifier int
*/
function m_transsend($conn, $identifier);
/**
* Wait x microsecs
* @return int
* @version PHP 4 >= 4.3.9, PHP 5
* @param $microsecs int
*/
function m_uwait($microsecs);
/**
* Whether or not to validate the passed identifier on any transaction it is passed to
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $conn resource
* @param $tf int
*/
function m_validateidentifier($conn, $tf);
/**
* Set whether or not to PING upon connect to verify connection
* @return bool
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $tf int
*/
function m_verifyconnection($conn, $tf);
/**
* Set whether or not to verify the server ssl certificate
* @return bool
* @version PHP 4 >= 4.3.9, PHP 5
* @param $conn resource
* @param $tf int
*/
function m_verifysslcert($conn, $tf);
/**
* Sort an array using a case insensitive "natural order" algorithm
* @return bool
* @version PHP 4, PHP 5
* @param &$array array
*/
function natcasesort(&$array);
/**
* Sort an array using a "natural order" algorithm
* @return bool
* @version PHP 4, PHP 5
* @param &$array array
*/
function natsort(&$array);
/**
* Add character at current position and advance cursor
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $ch int
*/
function ncurses_addch($ch);
/**
* Add attributed string with specified length at current position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $s string
* @param $n int
*/
function ncurses_addchnstr($s, $n);
/**
* Add attributed string at current position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $s string
*/
function ncurses_addchstr($s);
/**
* Add string with specified length at current position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $s string
* @param $n int
*/
function ncurses_addnstr($s, $n);
/**
* Output text at current position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $text string
*/
function ncurses_addstr($text);
/**
* Define default colors for color 0
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $fg int
* @param $bg int
*/
function ncurses_assume_default_colors($fg, $bg);
/**
* Turn off the given attributes
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $attributes int
*/
function ncurses_attroff($attributes);
/**
* Turn on the given attributes
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $attributes int
*/
function ncurses_attron($attributes);
/**
* Set given attributes
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $attributes int
*/
function ncurses_attrset($attributes);
/**
* Returns baudrate of terminal
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_baudrate();
/**
* Let the terminal beep
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_beep();
/**
* Set background property for terminal screen
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $attrchar int
*/
function ncurses_bkgd($attrchar);
/**
* Control screen background
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
* @param $attrchar int
*/
function ncurses_bkgdset($attrchar);
/**
* Draw a border around the screen using attributed characters
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $left int
* @param $right int
* @param $top int
* @param $bottom int
* @param $tl_corner int
* @param $tr_corner int
* @param $bl_corner int
* @param $br_corner int
*/
function ncurses_border($left, $right, $top, $bottom, $tl_corner, $tr_corner, $bl_corner, $br_corner);
/**
* Moves a visible panel to the bottom of the stack
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_bottom_panel($panel);
/**
* Check if we can change terminals colors
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_can_change_color();
/**
* Switch of input buffering
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_cbreak();
/**
* Clear screen
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_clear();
/**
* Clear screen from current position to bottom
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_clrtobot();
/**
* Clear screen from current position to end of line
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_clrtoeol();
/**
* Gets the RGB value for color
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $color int
* @param &$r int
* @param &$g int
* @param &$b int
*/
function ncurses_color_content($color, &$r, &$g, &$b);
/**
* Set fore- and background color
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $pair int
*/
function ncurses_color_set($pair);
/**
* Set cursor state
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $visibility int
*/
function ncurses_curs_set($visibility);
/**
* Define a keycode
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $definition string
* @param $keycode int
*/
function ncurses_define_key($definition, $keycode);
/**
* Saves terminals (program) mode
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_def_prog_mode();
/**
* Saves terminals (shell) mode
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_def_shell_mode();
/**
* Delay output on terminal using padding characters
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $milliseconds int
*/
function ncurses_delay_output($milliseconds);
/**
* Delete character at current position, move rest of line left
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_delch();
/**
* Delete line at current position, move rest of screen up
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_deleteln();
/**
* Delete a ncurses window
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $window resource
*/
function ncurses_delwin($window);
/**
* Remove panel from the stack and delete it (but not the associated window)
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_del_panel($panel);
/**
* Write all prepared refreshes to terminal
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_doupdate();
/**
* Activate keyboard input echo
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_echo();
/**
* Single character output including refresh
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $character int
*/
function ncurses_echochar($character);
/**
* Stop using ncurses, clean up the screen
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_end();
/**
* Erase terminal screen
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_erase();
/**
* Returns current erase character
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_erasechar();
/**
* Set LINES for iniscr() and newterm() to 1
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_filter();
/**
* Flash terminal screen (visual bell)
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_flash();
/**
* Flush keyboard input buffer
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_flushinp();
/**
* Read a character from keyboard
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_getch();
/**
* Returns the size of a window
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param &$y int
* @param &$x int
*/
function ncurses_getmaxyx($window, &$y, &$x);
/**
* Reads mouse event
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param &$mevent array
*/
function ncurses_getmouse(&$mevent);
/**
* Returns the current cursor position for a window
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param &$y int
* @param &$x int
*/
function ncurses_getyx($window, &$y, &$x);
/**
* Put terminal into halfdelay mode
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $tenth int
*/
function ncurses_halfdelay($tenth);
/**
* Check if terminal has colors
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_has_colors();
/**
* Check for insert- and delete-capabilities
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_has_ic();
/**
* Check for line insert- and delete-capabilities
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_has_il();
/**
* Check for presence of a function key on terminal keyboard
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $keycode int
*/
function ncurses_has_key($keycode);
/**
* Remove panel from the stack, making it invisible
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_hide_panel($panel);
/**
* Draw a horizontal line at current position using an attributed character and max. n characters long
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $charattr int
* @param $n int
*/
function ncurses_hline($charattr, $n);
/**
* Get character and attribute at current position
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_inch();
/**
* Initialize ncurses
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_init();
/**
* Set new RGB value for color
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $color int
* @param $r int
* @param $g int
* @param $b int
*/
function ncurses_init_color($color, $r, $g, $b);
/**
* Allocate a color pair
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $pair int
* @param $fg int
* @param $bg int
*/
function ncurses_init_pair($pair, $fg, $bg);
/**
* Insert character moving rest of line including character at current position
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $character int
*/
function ncurses_insch($character);
/**
* Insert lines before current line scrolling down (negative numbers delete and scroll up)
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $count int
*/
function ncurses_insdelln($count);
/**
* Insert a line, move rest of screen down
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_insertln();
/**
* Insert string at current position, moving rest of line right
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $text string
*/
function ncurses_insstr($text);
/**
* Reads string from terminal screen
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param &$buffer string
*/
function ncurses_instr(&$buffer);
/**
* Ncurses is in endwin mode, normal screen output may be performed
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_isendwin();
/**
* Enable or disable a keycode
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $keycode int
* @param $enable bool
*/
function ncurses_keyok($keycode, $enable);
/**
* Turns keypad on or off
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param $bf bool
*/
function ncurses_keypad($window, $bf);
/**
* Returns current line kill character
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_killchar();
/**
* Returns terminals description
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
*/
function ncurses_longname();
/**
* Enables/Disable 8-bit meta key information
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $8bit bool
*/
function ncurses_meta($window, $8bit);
/**
* Set timeout for mouse button clicks
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $milliseconds int
*/
function ncurses_mouseinterval($milliseconds);
/**
* Sets mouse options
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $newmask int
* @param &$oldmask int
*/
function ncurses_mousemask($newmask, &$oldmask);
/**
* Transforms coordinates
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param &$y int
* @param &$x int
* @param $toscreen bool
*/
function ncurses_mouse_trafo(&$y, &$x, $toscreen);
/**
* Move output position
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $y int
* @param $x int
*/
function ncurses_move($y, $x);
/**
* Moves a panel so that its upper-left corner is at [startx, starty]
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
* @param $startx int
* @param $starty int
*/
function ncurses_move_panel($panel, $startx, $starty);
/**
* Move current position and add character
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $c int
*/
function ncurses_mvaddch($y, $x, $c);
/**
* Move position and add attributed string with specified length
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $s string
* @param $n int
*/
function ncurses_mvaddchnstr($y, $x, $s, $n);
/**
* Move position and add attributed string
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $s string
*/
function ncurses_mvaddchstr($y, $x, $s);
/**
* Move position and add string with specified length
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $s string
* @param $n int
*/
function ncurses_mvaddnstr($y, $x, $s, $n);
/**
* Move position and add string
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $s string
*/
function ncurses_mvaddstr($y, $x, $s);
/**
* Move cursor immediately
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $old_y int
* @param $old_x int
* @param $new_y int
* @param $new_x int
*/
function ncurses_mvcur($old_y, $old_x, $new_y, $new_x);
/**
* Move position and delete character, shift rest of line left
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
*/
function ncurses_mvdelch($y, $x);
/**
* Move position and get character at new position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
*/
function ncurses_mvgetch($y, $x);
/**
* Set new position and draw a horizontal line using an attributed character and max. n characters long
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
* @param $attrchar int
* @param $n int
*/
function ncurses_mvhline($y, $x, $attrchar, $n);
/**
* Move position and get attributed character at new position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $y int
* @param $x int
*/
function ncurses_mvinch($y, $x);
/**
* Add string at new position in window
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param $y int
* @param $x int
* @param $text string
*/
function ncurses_mvwaddstr($window, $y, $x, $text);
/**
* Sleep
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $milliseconds int
*/
function ncurses_napms($milliseconds);
/**
* Creates a new pad (window)
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $rows int
* @param $cols int
*/
function ncurses_newpad($rows, $cols);
/**
* Create a new window
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $rows int
* @param $cols int
* @param $y int
* @param $x int
*/
function ncurses_newwin($rows, $cols, $y, $x);
/**
* Create a new panel and associate it with window
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
*/
function ncurses_new_panel($window);
/**
* Translate newline and carriage return / line feed
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_nl();
/**
* Switch terminal to cooked mode
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_nocbreak();
/**
* Switch off keyboard input echo
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_noecho();
/**
* Do not translate newline and carriage return / line feed
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_nonl();
/**
* Do not flush on signal characters
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_noqiflush();
/**
* Switch terminal out of raw mode
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_noraw();
/**
* Gets the RGB value for color
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $pair int
* @param &$f int
* @param &$b int
*/
function ncurses_pair_content($pair, &$f, &$b);
/**
* Returns the panel above panel
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_panel_above($panel);
/**
* Returns the panel below panel
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_panel_below($panel);
/**
* Returns the window associated with panel
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_panel_window($panel);
/**
* Copies a region from a pad into the virtual screen
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $pad resource
* @param $pminrow int
* @param $pmincol int
* @param $sminrow int
* @param $smincol int
* @param $smaxrow int
* @param $smaxcol int
*/
function ncurses_pnoutrefresh($pad, $pminrow, $pmincol, $sminrow, $smincol, $smaxrow, $smaxcol);
/**
* Copies a region from a pad into the virtual screen
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $pad resource
* @param $pminrow int
* @param $pmincol int
* @param $sminrow int
* @param $smincol int
* @param $smaxrow int
* @param $smaxcol int
*/
function ncurses_prefresh($pad, $pminrow, $pmincol, $sminrow, $smincol, $smaxrow, $smaxcol);
/**
* Apply padding information to the string and output it
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $text string
*/
function ncurses_putp($text);
/**
* Flush on signal characters
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_qiflush();
/**
* Switch terminal into raw mode
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_raw();
/**
* Refresh screen
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $ch int
*/
function ncurses_refresh($ch);
/**
* Replaces the window associated with panel
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
* @param $window resource
*/
function ncurses_replace_panel($panel, $window);
/**
* Restores saved terminal state
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_resetty();
/**
* Resets the prog mode saved by def_prog_mode
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ncurses_reset_prog_mode();
/**
* Resets the shell mode saved by def_shell_mode
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ncurses_reset_shell_mode();
/**
* Saves terminal state
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_savetty();
/**
* Scroll window content up or down without changing current position
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $count int
*/
function ncurses_scrl($count);
/**
* Dump screen content to file
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
*/
function ncurses_scr_dump($filename);
/**
* Initialize screen from file dump
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
*/
function ncurses_scr_init($filename);
/**
* Restore screen from file dump
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
*/
function ncurses_scr_restore($filename);
/**
* Inherit screen from file dump
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $filename string
*/
function ncurses_scr_set($filename);
/**
* Places an invisible panel on top of the stack, making it visible
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_show_panel($panel);
/**
* Returns current soft label key attribute
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_attr();
/**
* Turn off the given attributes for soft function-key labels
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $intarg int
*/
function ncurses_slk_attroff($intarg);
/**
* Turn on the given attributes for soft function-key labels
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $intarg int
*/
function ncurses_slk_attron($intarg);
/**
* Set given attributes for soft function-key labels
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $intarg int
*/
function ncurses_slk_attrset($intarg);
/**
* Clears soft labels from screen
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_clear();
/**
* Sets color for soft label keys
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $intarg int
*/
function ncurses_slk_color($intarg);
/**
* Initializes soft label key functions
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $format int
*/
function ncurses_slk_init($format);
/**
* Copies soft label keys to virtual screen
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_noutrefresh();
/**
* Copies soft label keys to screen
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_refresh();
/**
* Restores soft label keys
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_restore();
/**
* Sets function key labels
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $labelnr int
* @param $label string
* @param $format int
*/
function ncurses_slk_set($labelnr, $label, $format);
/**
* Forces output when ncurses_slk_noutrefresh is performed
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_slk_touch();
/**
* Stop using 'standout' attribute
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_standend();
/**
* Start using 'standout' attribute
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_standout();
/**
* Start using colors
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_start_color();
/**
* Returns a logical OR of all attribute flags supported by terminal
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_termattrs();
/**
* Returns terminals (short)-name
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
*/
function ncurses_termname();
/**
* Set timeout for special key sequences
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
* @param $millisec int
*/
function ncurses_timeout($millisec);
/**
* Moves a visible panel to the top of the stack
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $panel resource
*/
function ncurses_top_panel($panel);
/**
* Specify different filedescriptor for typeahead checking
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $fd int
*/
function ncurses_typeahead($fd);
/**
* Put a character back into the input stream
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $keycode int
*/
function ncurses_ungetch($keycode);
/**
* Pushes mouse event to queue
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $mevent array
*/
function ncurses_ungetmouse($mevent);
/**
* Refreshes the virtual screen to reflect the relations between panels in the stack
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ncurses_update_panels();
/**
* Assign terminal default colors to color id -1
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
*/
function ncurses_use_default_colors();
/**
* Control use of environment information about terminal size
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
* @param $flag bool
*/
function ncurses_use_env($flag);
/**
* Control use of extended names in terminfo descriptions
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $flag bool
*/
function ncurses_use_extended_names($flag);
/**
* Display the string on the terminal in the video attribute mode
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $intarg int
*/
function ncurses_vidattr($intarg);
/**
* Draw a vertical line at current position using an attributed character and max. n characters long
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $charattr int
* @param $n int
*/
function ncurses_vline($charattr, $n);
/**
* Adds character at current position in a window and advance cursor
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $ch int
*/
function ncurses_waddch($window, $ch);
/**
* Outputs text at current postion in window
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param $str string
* @param $n (optional) int
*/
function ncurses_waddstr($window, $str, $n);
/**
* Turns off attributes for a window
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $attrs int
*/
function ncurses_wattroff($window, $attrs);
/**
* Turns on attributes for a window
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $attrs int
*/
function ncurses_wattron($window, $attrs);
/**
* Set the attributes for a window
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $attrs int
*/
function ncurses_wattrset($window, $attrs);
/**
* Draws a border around the window using attributed characters
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $left int
* @param $right int
* @param $top int
* @param $bottom int
* @param $tl_corner int
* @param $tr_corner int
* @param $bl_corner int
* @param $br_corner int
*/
function ncurses_wborder($window, $left, $right, $top, $bottom, $tl_corner, $tr_corner, $bl_corner, $br_corner);
/**
* Clears window
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
*/
function ncurses_wclear($window);
/**
* Sets windows color pairings
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param $color_pair int
*/
function ncurses_wcolor_set($window, $color_pair);
/**
* Erase window contents
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
*/
function ncurses_werase($window);
/**
* Reads a character from keyboard (window)
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
*/
function ncurses_wgetch($window);
/**
* Draws a horizontal line in a window at current position using an attributed character and max. n characters long
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $charattr int
* @param $n int
*/
function ncurses_whline($window, $charattr, $n);
/**
* Transforms window/stdscr coordinates
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param &$y int
* @param &$x int
* @param $toscreen bool
*/
function ncurses_wmouse_trafo($window, &$y, &$x, $toscreen);
/**
* Moves windows output position
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
* @param $y int
* @param $x int
*/
function ncurses_wmove($window, $y, $x);
/**
* Copies window to virtual screen
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
*/
function ncurses_wnoutrefresh($window);
/**
* Refresh window on terminal screen
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $window resource
*/
function ncurses_wrefresh($window);
/**
* End standout mode for a window
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
*/
function ncurses_wstandend($window);
/**
* Enter standout mode for a window
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
*/
function ncurses_wstandout($window);
/**
* Draws a vertical line in a window at current position using an attributed character and max. n characters long
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $window resource
* @param $charattr int
* @param $n int
*/
function ncurses_wvline($window, $charattr, $n);
/**
* Send a beep to the terminal
* @return 
* @version PECL
*/
function newt_bell();
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
*/
function newt_button($left, $top, $text);
/**
* --
* @return resource
* @version PECL
* @param &$buttons array
*/
function newt_button_bar(&$buttons);
/**
* Open a centered window of the specified size
* @return int
* @version PECL
* @param $width int
* @param $height int
* @param $title (optional) string
*/
function newt_centered_window($width, $height, $title);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
* @param $def_value string
* @param $seq (optional) string
*/
function newt_checkbox($left, $top, $text, $def_value, $seq);
/**
* --
* @return string
* @version PECL
* @param $checkbox resource
*/
function newt_checkbox_get_value($checkbox);
/**
* --
* @return 
* @version PECL
* @param $checkbox resource
* @param $flags int
* @param $sense int
*/
function newt_checkbox_set_flags($checkbox, $flags, $sense);
/**
* --
* @return 
* @version PECL
* @param $checkbox resource
* @param $value string
*/
function newt_checkbox_set_value($checkbox, $value);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $height int
* @param $flags (optional) int
*/
function newt_checkbox_tree($left, $top, $height, $flags);
/**
* *
* @return 
* @version PECL
* @param $checkboxtree resource
* @param $text string
* @param $data mixed
* @param $flags int
* @param $index int
* @param $params1 (optional) int
*/
function newt_checkbox_tree_add_item($checkboxtree, $text, $data, $flags, $index, $params1);
/**
* --
* @return array
* @version PECL
* @param $checkboxtree resource
* @param $data mixed
*/
function newt_checkbox_tree_find_item($checkboxtree, $data);
/**
* --
* @return mixed
* @version PECL
* @param $checkboxtree resource
*/
function newt_checkbox_tree_get_current($checkboxtree);
/**
* --
* @return string
* @version PECL
* @param $checkboxtree resource
* @param $data mixed
*/
function newt_checkbox_tree_get_entry_value($checkboxtree, $data);
/**
* --
* @return array
* @version PECL
* @param $checkboxtree resource
* @param $seqnum string
*/
function newt_checkbox_tree_get_multi_selection($checkboxtree, $seqnum);
/**
* --
* @return array
* @version PECL
* @param $checkboxtree resource
*/
function newt_checkbox_tree_get_selection($checkboxtree);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $height int
* @param $seq string
* @param $flags (optional) int
*/
function newt_checkbox_tree_multi($left, $top, $height, $seq, $flags);
/**
* --
* @return 
* @version PECL
* @param $checkboxtree resource
* @param $data mixed
*/
function newt_checkbox_tree_set_current($checkboxtree, $data);
/**
* --
* @return 
* @version PECL
* @param $checkboxtree resource
* @param $data mixed
* @param $text string
*/
function newt_checkbox_tree_set_entry($checkboxtree, $data, $text);
/**
* --
* @return 
* @version PECL
* @param $checkboxtree resource
* @param $data mixed
* @param $value string
*/
function newt_checkbox_tree_set_entry_value($checkboxtree, $data, $value);
/**
* --
* @return 
* @version PECL
* @param $checkbox_tree resource
* @param $width int
*/
function newt_checkbox_tree_set_width($checkbox_tree, $width);
/**
* Discards the contents of the terminal's input buffer without waiting for additional input
* @return 
* @version PECL
*/
function newt_clear_key_buffer();
/**
* --
* @return 
* @version PECL
*/
function newt_cls();
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
*/
function newt_compact_button($left, $top, $text);
/**
* --
* @return 
* @version PECL
* @param $component resource
* @param $func_name mixed
* @param $data mixed
*/
function newt_component_add_callback($component, $func_name, $data);
/**
* --
* @return 
* @version PECL
* @param $component resource
* @param $takes_focus bool
*/
function newt_component_takes_focus($component, $takes_focus);
/**
* --
* @return resource
* @version PECL
* @param $cols int
* @param $rows int
*/
function newt_create_grid($cols, $rows);
/**
* --
* @return 
* @version PECL
*/
function newt_cursor_off();
/**
* --
* @return 
* @version PECL
*/
function newt_cursor_on();
/**
* --
* @return 
* @version PECL
* @param $microseconds int
*/
function newt_delay($microseconds);
/**
* --
* @return 
* @version PECL
* @param $form resource
*/
function newt_draw_form($form);
/**
* Displays the string text at the position indicated
* @return 
* @version PECL
* @param $left int
* @param $top int
* @param $text string
*/
function newt_draw_root_text($left, $top, $text);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $width int
* @param $init_value (optional) string
* @param $flags (optional) int
*/
function newt_entry($left, $top, $width, $init_value, $flags);
/**
* --
* @return string
* @version PECL
* @param $entry resource
*/
function newt_entry_get_value($entry);
/**
* --
* @return 
* @version PECL
* @param $entry resource
* @param $value string
* @param $cursor_at_end (optional) bool
*/
function newt_entry_set($entry, $value, $cursor_at_end);
/**
* --
* @return 
* @version PECL
* @param $entry resource
* @param $filter callback
* @param $data mixed
*/
function newt_entry_set_filter($entry, $filter, $data);
/**
* --
* @return 
* @version PECL
* @param $entry resource
* @param $flags int
* @param $sense int
*/
function newt_entry_set_flags($entry, $flags, $sense);
/**
* Uninitializes newt interface
* @return int
* @version PECL
*/
function newt_finished();
/**
* Create a form
* @return resource
* @version PECL
* @param $vert_bar resource
* @param $help (optional) string
* @param $flags (optional) int
*/
function newt_form($vert_bar, $help, $flags);
/**
* Adds a single component to the form
* @return 
* @version PECL
* @param $form resource
* @param $component resource
*/
function newt_form_add_component($form, $component);
/**
* Add several components to the form
* @return 
* @version PECL
* @param $form resource
* @param $components array
*/
function newt_form_add_components($form, $components);
/**
* Destroys a form
* @return 
* @version PECL
* @param $form resource
*/
function newt_form_destroy($form);
/**
* --
* @return resource
* @version PECL
* @param $form resource
*/
function newt_form_get_current($form);
/**
* Runs a form
* @return 
* @version PECL
* @param $form resource
* @param &$exit_struct array
*/
function newt_form_run($form, &$exit_struct);
/**
* --
* @return 
* @version PECL
* @param $from resource
* @param $background int
*/
function newt_form_set_background($from, $background);
/**
* --
* @return 
* @version PECL
* @param $form resource
* @param $height int
*/
function newt_form_set_height($form, $height);
/**
* --
* @return 
* @version PECL
* @param $form resource
*/
function newt_form_set_size($form);
/**
* --
* @return 
* @version PECL
* @param $form resource
* @param $milliseconds int
*/
function newt_form_set_timer($form, $milliseconds);
/**
* --
* @return 
* @version PECL
* @param $form resource
* @param $width int
*/
function newt_form_set_width($form, $width);
/**
* --
* @return 
* @version PECL
* @param $form resource
* @param $stream resource
* @param $flags (optional) int
*/
function newt_form_watch_fd($form, $stream, $flags);
/**
* Fills in the passed references with the current size of the terminal
* @return 
* @version PECL
* @param &$cols int
* @param &$rows int
*/
function newt_get_screen_size(&$cols, &$rows);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $form resource
* @param $recurse bool
*/
function newt_grid_add_components_to_form($grid, $form, $recurse);
/**
* --
* @return resource
* @version PECL
* @param $text resource
* @param $middle resource
* @param $buttons resource
*/
function newt_grid_basic_window($text, $middle, $buttons);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $recurse bool
*/
function newt_grid_free($grid, $recurse);
/**
* --
* @return 
* @version PECL
* @param $grid resouce
* @param &$width int
* @param &$height int
*/
function newt_grid_get_size($grid, &$width, &$height);
/**
* --
* @return resource
* @version PECL
* @param $element1_type int
* @param $element1 resource
* @param $params1 (optional) int
* @param $params2 (optional) resource
*/
function newt_grid_h_close_stacked($element1_type, $element1, $params1, $params2);
/**
* --
* @return resource
* @version PECL
* @param $element1_type int
* @param $element1 resource
* @param $params1 (optional) int
* @param $params2 (optional) resource
*/
function newt_grid_h_stacked($element1_type, $element1, $params1, $params2);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $left int
* @param $top int
*/
function newt_grid_place($grid, $left, $top);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $col int
* @param $row int
* @param $type int
* @param $val resource
* @param $pad_left int
* @param $pad_top int
* @param $pad_right int
* @param $pad_bottom int
* @param $anchor int
* @param $flags (optional) int
*/
function newt_grid_set_field($grid, $col, $row, $type, $val, $pad_left, $pad_top, $pad_right, $pad_bottom, $anchor, $flags);
/**
* --
* @return resource
* @version PECL
* @param $text resource
* @param $middle resource
* @param $buttons resource
*/
function newt_grid_simple_window($text, $middle, $buttons);
/**
* --
* @return resource
* @version PECL
* @param $element1_type int
* @param $element1 resource
* @param $params1 (optional) int
* @param $params2 (optional) resource
*/
function newt_grid_v_close_stacked($element1_type, $element1, $params1, $params2);
/**
* --
* @return resource
* @version PECL
* @param $element1_type int
* @param $element1 resource
* @param $params1 (optional) int
* @param $params2 (optional) resource
*/
function newt_grid_v_stacked($element1_type, $element1, $params1, $params2);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $title string
*/
function newt_grid_wrapped_window($grid, $title);
/**
* --
* @return 
* @version PECL
* @param $grid resource
* @param $title string
* @param $left int
* @param $top int
*/
function newt_grid_wrapped_window_at($grid, $title, $left, $top);
/**
* Initialize newt
* @return int
* @version PECL
*/
function newt_init();
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
*/
function newt_label($left, $top, $text);
/**
* --
* @return 
* @version PECL
* @param $label resource
* @param $text string
*/
function newt_label_set_text($label, $text);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $height int
* @param $flags (optional) int
*/
function newt_listbox($left, $top, $height, $flags);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $text string
* @param $data mixed
*/
function newt_listbox_append_entry($listbox, $text, $data);
/**
* --
* @return 
* @version PECL
* @param $listobx resource
*/
function newt_listbox_clear($listobx);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
*/
function newt_listbox_clear_selection($listbox);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $key mixed
*/
function newt_listbox_delete_entry($listbox, $key);
/**
* --
* @return string
* @version PECL
* @param $listbox resource
*/
function newt_listbox_get_current($listbox);
/**
* --
* @return array
* @version PECL
* @param $listbox resource
*/
function newt_listbox_get_selection($listbox);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $text string
* @param $data mixed
* @param $key mixed
*/
function newt_listbox_insert_entry($listbox, $text, $data, $key);
/**
* --
* @return int
* @version PECL
* @param $listbox resource
*/
function newt_listbox_item_count($listbox);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $key mixed
* @param $sense int
*/
function newt_listbox_select_item($listbox, $key, $sense);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $num int
*/
function newt_listbox_set_current($listbox, $num);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $key mixed
*/
function newt_listbox_set_current_by_key($listbox, $key);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $num int
* @param $data mixed
*/
function newt_listbox_set_data($listbox, $num, $data);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $num int
* @param $text string
*/
function newt_listbox_set_entry($listbox, $num, $text);
/**
* --
* @return 
* @version PECL
* @param $listbox resource
* @param $width int
*/
function newt_listbox_set_width($listbox, $width);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
* @param $is_default bool
* @param $prev_item resouce
* @param $data mixed
* @param $flags (optional) int
*/
function newt_listitem($left, $top, $text, $is_default, $prev_item, $data, $flags);
/**
* --
* @return mixed
* @version PECL
* @param $item resource
*/
function newt_listitem_get_data($item);
/**
* --
* @return 
* @version PECL
* @param $item resource
* @param $text string
*/
function newt_listitem_set($item, $text);
/**
* Open a window of the specified size and position
* @return int
* @version PECL
* @param $left int
* @param $top int
* @param $width int
* @param $height int
* @param $title (optional) string
*/
function newt_open_window($left, $top, $width, $height, $title);
/**
* Replaces the current help line with the one from the stack
* @return 
* @version PECL
*/
function newt_pop_help_line();
/**
* Removes the top window from the display
* @return 
* @version PECL
*/
function newt_pop_window();
/**
* Saves the current help line on a stack, and displays the new line
* @return 
* @version PECL
* @param $text string
*/
function newt_push_help_line($text);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $text string
* @param $is_default bool
* @param $prev_button (optional) resource
*/
function newt_radiobutton($left, $top, $text, $is_default, $prev_button);
/**
* --
* @return resource
* @version PECL
* @param $set_member resource
*/
function newt_radio_get_current($set_member);
/**
* --
* @return 
* @version PECL
*/
function newt_redraw_help_line();
/**
* --
* @return string
* @version PECL
* @param $text string
* @param $width int
* @param $flex_down int
* @param $flex_up int
* @param &$actual_width int
* @param &$actual_height int
*/
function newt_reflow_text($text, $width, $flex_down, $flex_up, &$actual_width, &$actual_height);
/**
* Updates modified portions of the screen
* @return 
* @version PECL
*/
function newt_refresh();
/**
* --
* @return 
* @version PECL
* @param $redraw bool
*/
function newt_resize_screen($redraw);
/**
* Resume using the newt interface after calling newt_suspend()
* @return 
* @version PECL
*/
function newt_resume();
/**
* Runs a form
* @return resource
* @version PECL
* @param $form resource
*/
function newt_run_form($form);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $width int
* @param $full_value int
*/
function newt_scale($left, $top, $width, $full_value);
/**
* --
* @return 
* @version PECL
* @param $scale resource
* @param $amount int
*/
function newt_scale_set($scale, $amount);
/**
* --
* @return 
* @version PECL
* @param $scrollbar resource
* @param $where int
* @param $total int
*/
function newt_scrollbar_set($scrollbar, $where, $total);
/**
* --
* @return 
* @version PECL
* @param $function mixed
*/
function newt_set_help_callback($function);
/**
* Set a callback function which gets invoked when user presses the suspend key
* @return 
* @version PECL
* @param $function callback
* @param $data mixed
*/
function newt_set_suspend_callback($function, $data);
/**
* Tells newt to return the terminal to its initial state
* @return 
* @version PECL
*/
function newt_suspend();
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $width int
* @param $height int
* @param $flags (optional) int
*/
function newt_textbox($left, $top, $width, $height, $flags);
/**
* --
* @return int
* @version PECL
* @param $textbox resource
*/
function newt_textbox_get_num_lines($textbox);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $*text char
* @param $width int
* @param $flex_down int
* @param $flex_up int
* @param $flags (optional) int
*/
function newt_textbox_reflowed($left, $top, $*text, $width, $flex_down, $flex_up, $flags);
/**
* --
* @return 
* @version PECL
* @param $textbox resource
* @param $height int
*/
function newt_textbox_set_height($textbox, $height);
/**
* --
* @return 
* @version PECL
* @param $textbox resource
* @param $text string
*/
function newt_textbox_set_text($textbox, $text);
/**
* --
* @return resource
* @version PECL
* @param $left int
* @param $top int
* @param $height int
* @param $normal_colorset (optional) int
* @param $thumb_colorset (optional) int
*/
function newt_vertical_scrollbar($left, $top, $height, $normal_colorset, $thumb_colorset);
/**
* Doesn't return until a key has been pressed
* @return 
* @version PECL
*/
function newt_wait_for_key();
/**
* --
* @return int
* @version PECL
* @param $title string
* @param $button1_text string
* @param $button2_text string
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function newt_win_choice($title, $button1_text, $button2_text, $format, $args, $params1);
/**
* --
* @return int
* @version PECL
* @param $title string
* @param $text string
* @param $suggested_width int
* @param $flex_down int
* @param $flex_up int
* @param $data_width int
* @param &$items array
* @param $button1 string
* @param $params1 (optional) string
*/
function newt_win_entries($title, $text, $suggested_width, $flex_down, $flex_up, $data_width, &$items, $button1, $params1);
/**
* --
* @return int
* @version PECL
* @param $title string
* @param $text string
* @param $suggestedWidth int
* @param $flexDown int
* @param $flexUp int
* @param $maxListHeight int
* @param $items array
* @param &$listItem int
* @param $button1 (optional) string
* @param $params1 (optional) string
*/
function newt_win_menu($title, $text, $suggestedWidth, $flexDown, $flexUp, $maxListHeight, $items, &$listItem, $button1, $params1);
/**
* --
* @return 
* @version PECL
* @param $title string
* @param $button_text string
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function newt_win_message($title, $button_text, $format, $args, $params1);
/**
* --
* @return 
* @version PECL
* @param $title string
* @param $button_text string
* @param $format string
* @param $args array
*/
function newt_win_messagev($title, $button_text, $format, $args);
/**
* --
* @return int
* @version PECL
* @param $title string
* @param $button1_text string
* @param $button2_text string
* @param $button3_text string
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function newt_win_ternary($title, $button1_text, $button2_text, $button3_text, $format, $args, $params1);
/**
* Advance the internal array pointer of an array
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function next(&$array);
/**
* Plural version of gettext
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $msgid1 string
* @param $msgid2 string
* @param $n int
*/
function ngettext($msgid1, $msgid2, $n);
/**
* Inserts HTML line breaks before all newlines in a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function nl2br($string);
/**
* Query language and locale information
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $item int
*/
function nl_langinfo($item);
/**
* Open the message msg_number in the specified mailbox on the specified server (leave serv
* @return array
* @version PHP 4 >= 4.0.5, PECL
* @param $server string
* @param $mailbox string
* @param $msg_number int
*/
function notes_body($server, $mailbox, $msg_number);
/**
* Copy a Lotus Notes database
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $from_database_name string
* @param $to_database_name string
*/
function notes_copy_db($from_database_name, $to_database_name);
/**
* Create a Lotus Notes database
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
*/
function notes_create_db($database_name);
/**
* Create a note using form form_name
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $form_name string
*/
function notes_create_note($database_name, $form_name);
/**
* Drop a Lotus Notes database
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
*/
function notes_drop_db($database_name);
/**
* Returns a note id found in database_name
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $name string
* @param $type (optional) string
*/
function notes_find_note($database_name, $name, $type);
/**
* Open the message msg_number in the specified mailbox on the specified server (leave serv
* @return object
* @version PHP 4 >= 4.0.5, PECL
* @param $server string
* @param $mailbox string
* @param $msg_number int
*/
function notes_header_info($server, $mailbox, $msg_number);
/**
* Returns the notes from a selected database_name
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $db string
*/
function notes_list_msgs($db);
/**
* Mark a note_id as read for the User user_name
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $user_name string
* @param $note_id string
*/
function notes_mark_read($database_name, $user_name, $note_id);
/**
* Mark a note_id as unread for the User user_name
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $user_name string
* @param $note_id string
*/
function notes_mark_unread($database_name, $user_name, $note_id);
/**
* Create a navigator name, in database_name
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $name string
*/
function notes_nav_create($database_name, $name);
/**
* Find notes that match keywords in database_name
* @return array
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $keywords string
*/
function notes_search($database_name, $keywords);
/**
* Returns the unread note id's for the current User user_name
* @return array
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
* @param $user_name string
*/
function notes_unread($database_name, $user_name);
/**
* Get the version Lotus Notes
* @return float
* @version PHP 4 >= 4.0.5, PECL
* @param $database_name string
*/
function notes_version($database_name);
/**
* Fetch all HTTP request headers
* @return array
* @version PHP 4 >= 4.3.3, PHP 5
*/
function nsapi_request_headers();
/**
* Fetch all HTTP response headers
* @return array
* @version PHP 4 >= 4.3.3, PHP 5
*/
function nsapi_response_headers();
/**
* Perform an NSAPI sub-request
* @return bool
* @version PHP 4 >= 4.3.3, PHP 5
* @param $uri string
*/
function nsapi_virtual($uri);
/**
* Format a number with grouped thousands
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $number float
* @param $decimals (optional) int
* @param $dec_point (optional) string
* @param $thousands_sep (optional) string
*/
function number_format($number, $decimals, $dec_point, $thousands_sep);
/**
* Clean (erase) the output buffer
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
*/
function ob_clean();
/**
* Clean (erase) the output buffer and turn off output buffering
* @return bool
* @version PHP 4, PHP 5
*/
function ob_end_clean();
/**
* Flush (send) the output buffer and turn off output buffering
* @return bool
* @version PHP 4, PHP 5
*/
function ob_end_flush();
/**
* ETag output handler
* @return string
* @version PECL
* @param $data string
* @param $mode int
*/
function ob_etaghandler($data, $mode);
/**
* Flush (send) the output buffer
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
*/
function ob_flush();
/**
* Get current buffer contents and delete current output buffer
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ob_get_clean();
/**
* Return the contents of the output buffer
* @return string
* @version PHP 4, PHP 5
*/
function ob_get_contents();
/**
* Flush the output buffer, return it as a string and turn off output buffering
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ob_get_flush();
/**
* Return the length of the output buffer
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
*/
function ob_get_length();
/**
* Return the nesting level of the output buffering mechanism
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
*/
function ob_get_level();
/**
* Get status of output buffers
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $full_status=FALSE bool
*/
function ob_get_status($full_status=FALSE);
/**
* ob_start callback function to gzip output buffer
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $buffer string
* @param $mode int
*/
function ob_gzhandler($buffer, $mode);
/**
* Convert character encoding as output buffer handler
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $contents string
* @param $status int
*/
function ob_iconv_handler($contents, $status);
/**
* Turn implicit flush on/off
* @return 
* @version PHP 4, PHP 5
* @param $flag int
*/
function ob_implicit_flush($flag);
/**
* List all output handlers in use
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
*/
function ob_list_handlers();
/**
* Turn on output buffering
* @return bool
* @version PHP 4, PHP 5
* @param $output_callback callback
* @param $chunk_size (optional) int
* @param $erase (optional) bool
*/
function ob_start($output_callback, $chunk_size, $erase);
/**
* ob_start callback function to repair the buffer
* @return string
* @version PHP 5
* @param $input string
* @param $mode (optional) int
*/
function ob_tidyhandler($input, $mode);
/**
* Alias of oci_bind_by_name()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocibindbyname();
/**
* Alias of oci_cancel()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ocicancel();
/**
* Alias of oci_field_is_null()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocicolumnisnull();
/**
* Alias of oci_field_name()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocicolumnname();
/**
* Alias of oci_field_precision()
* @return &#13;
* @version PHP 4, PHP 5
*/
function ocicolumnprecision();
/**
* Alias of oci_field_scale()
* @return &#13;
* @version PHP 4, PHP 5
*/
function ocicolumnscale();
/**
* Alias of oci_field_size()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocicolumnsize();
/**
* Alias of oci_field_type()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocicolumntype();
/**
* Alias of oci_field_type_raw()
* @return &#13;
* @version PHP 4, PHP 5
*/
function ocicolumntyperaw();
/**
* Alias of oci_commit()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocicommit();
/**
* Alias of oci_define_by_name()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocidefinebyname();
/**
* Alias of oci_error()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocierror();
/**
* Alias of oci_execute()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ociexecute();
/**
* Alias of oci_fetch()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocifetch();
/**
* Fetches the next row into an array
* @return int
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $statement resource
* @param &$result array
* @param $mode (optional) int
*/
function ocifetchinto($statement, &$result, $mode);
/**
* Alias of oci_fetch_all()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ocifetchstatement();
/**
* Alias of oci_free_statement()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ocifreecursor();
/**
* Alias of oci_free_statement()
* @return &#13;
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
*/
function ocifreestatement();
/**
* Alias of oci_internal_debug()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ociinternaldebug();
/**
* Alias of oci_close()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocilogoff();
/**
* Alias of oci_connect()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocilogon();
/**
* Alias of oci_new_collection()
* @return &#13;
* @version PHP 4 >= 4.0.6, PHP 5
*/
function ocinewcollection();
/**
* Alias of oci_new_cursor()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ocinewcursor();
/**
* Alias of oci_new_descriptor()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocinewdescriptor();
/**
* Alias of oci_new_connect()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ocinlogon();
/**
* Alias of oci_num_fields()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ocinumcols();
/**
* Alias of oci_parse()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ociparse();
/**
* Alias of oci_pconnect()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function ociplogon();
/**
* Alias of oci_result()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ociresult();
/**
* Alias of oci_rollback()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocirollback();
/**
* Alias of oci_num_rows()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function ocirowcount();
/**
* Alias of oci_server_version()
* @return &#13;
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
*/
function ociserverversion();
/**
* Alias of oci_set_prefetch()
* @return &#13;
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
*/
function ocisetprefetch();
/**
* Alias of oci_statement_type()
* @return &#13;
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
*/
function ocistatementtype();
/**
* Binds the PHP variable to the Oracle placeholder
* @return bool
* @version PHP 5
* @param $stmt resource
* @param $ph_name string
* @param &$variable mixed
* @param $maxlength (optional) int
* @param $type (optional) int
*/
function oci_bind_by_name($stmt, $ph_name, &$variable, $maxlength, $type);
/**
* Cancels reading from cursor
* @return bool
* @version PHP 5
* @param $stmt resource
*/
function oci_cancel($stmt);
/**
* Closes Oracle connection
* @return bool
* @version PHP 5
* @param $connection resource
*/
function oci_close($connection);
/**
* Commits outstanding statements
* @return bool
* @version PHP 5
* @param $connection resource
*/
function oci_commit($connection);
/**
* Establishes a connection to the Oracle server
* @return resource
* @version PHP 5
* @param $username string
* @param $password string
* @param $db (optional) string
* @param $charset (optional) string
* @param $session_mode (optional) int
*/
function oci_connect($username, $password, $db, $charset, $session_mode);
/**
* Uses a PHP variable for the define-step during a SELECT
* @return bool
* @version PHP 5
* @param $statement resource
* @param $column_name string
* @param &$variable mixed
* @param $type (optional) int
*/
function oci_define_by_name($statement, $column_name, &$variable, $type);
/**
* Returns the last error found
* @return array
* @version PHP 5
* @param $source resource
*/
function oci_error($source);
/**
* Executes a statement
* @return bool
* @version PHP 5
* @param $stmt resource
* @param $mode (optional) int
*/
function oci_execute($stmt, $mode);
/**
* Fetches the next row into result-buffer
* @return bool
* @version PHP 5
* @param $statement resource
*/
function oci_fetch($statement);
/**
* Fetches all rows of result data into an array
* @return int
* @version PHP 5
* @param $statement resource
* @param &$output array
* @param $skip (optional) int
* @param $maxrows (optional) int
* @param $flags (optional) int
*/
function oci_fetch_all($statement, &$output, $skip, $maxrows, $flags);
/**
* Returns the next row from the result data as an associative or numeric array, or both
* @return array
* @version PHP 5
* @param $statement resource
* @param $mode (optional) int
*/
function oci_fetch_array($statement, $mode);
/**
* Returns the next row from the result data as an associative array
* @return array
* @version PHP 5
* @param $statement resource
*/
function oci_fetch_assoc($statement);
/**
* Returns the next row from the result data as an object
* @return object
* @version PHP 5
* @param $statement resource
*/
function oci_fetch_object($statement);
/**
* Returns the next row from the result data as a numeric array
* @return array
* @version PHP 5
* @param $statement resource
*/
function oci_fetch_row($statement);
/**
* Checks if the field is NULL
* @return bool
* @version PHP 5
* @param $stmt resource
* @param $field mixed
*/
function oci_field_is_null($stmt, $field);
/**
* Returns the name of a field from the statement
* @return string
* @version PHP 5
* @param $statement resource
* @param $field int
*/
function oci_field_name($statement, $field);
/**
* Tell the precision of a field
* @return int
* @version PHP 5
* @param $statement resource
* @param $field int
*/
function oci_field_precision($statement, $field);
/**
* Tell the scale of the field
* @return int
* @version PHP 5
* @param $statement resource
* @param $field int
*/
function oci_field_scale($statement, $field);
/**
* Returns field's size
* @return int
* @version PHP 5
* @param $stmt resource
* @param $field mixed
*/
function oci_field_size($stmt, $field);
/**
* Returns field's data type
* @return mixed
* @version PHP 5
* @param $stmt resource
* @param $field int
*/
function oci_field_type($stmt, $field);
/**
* Tell the raw Oracle data type of the field
* @return int
* @version PHP 5
* @param $statement resource
* @param $field int
*/
function oci_field_type_raw($statement, $field);
/**
* Frees all resources associated with statement or cursor
* @return bool
* @version PHP 5
* @param $statement resource
*/
function oci_free_statement($statement);
/**
* Enables or disables internal debug output
* @return 
* @version PHP 5
* @param $onoff int
*/
function oci_internal_debug($onoff);
/**
* Copies large object
* @return bool
* @version PHP 5
* @param $lob_to OCI-Lob
* @param $lob_from OCI-Lob
* @param $length (optional) int
*/
function oci_lob_copy($lob_to, $lob_from, $length);
/**
* Compares two LOB/FILE locators for equality
* @return bool
* @version PHP 5
* @param $lob1 OCI-Lob
* @param $lob2 OCI-Lob
*/
function oci_lob_is_equal($lob1, $lob2);
/**
* Allocates new collection object
* @return OCI-Collection
* @version PHP 5
* @param $connection resource
* @param $tdo string
* @param $schema (optional) string
*/
function oci_new_collection($connection, $tdo, $schema);
/**
* Establishes a new connection to the Oracle server
* @return resource
* @version PHP 5
* @param $username string
* @param $password string
* @param $db (optional) string
* @param $charset (optional) string
* @param $session_mode (optional) int
*/
function oci_new_connect($username, $password, $db, $charset, $session_mode);
/**
* Allocates and returns a new cursor (statement handle)
* @return resource
* @version PHP 5
* @param $connection resource
*/
function oci_new_cursor($connection);
/**
* Initializes a new empty LOB or FILE descriptor
* @return OCI-Lob
* @version PHP 5
* @param $connection resource
* @param $type (optional) int
*/
function oci_new_descriptor($connection, $type);
/**
* Returns the number of result columns in a statement
* @return int
* @version PHP 5
* @param $statement resource
*/
function oci_num_fields($statement);
/**
* Returns number of rows affected during statement execution
* @return int
* @version PHP 5
* @param $stmt resource
*/
function oci_num_rows($stmt);
/**
* Prepares Oracle statement for execution
* @return resource
* @version PHP 5
* @param $connection resource
* @param $query string
*/
function oci_parse($connection, $query);
/**
* Changes password of Oracle's user
* @return bool
* @version PHP 5
* @param $connection resource
* @param $username string
* @param $old_password string
* @param $new_password string
*/
function oci_password_change($connection, $username, $old_password, $new_password);
/**
* Connect to an Oracle database using a persistent connection
* @return resource
* @version PHP 5
* @param $username string
* @param $password string
* @param $db (optional) string
* @param $charset (optional) string
* @param $session_mode (optional) int
*/
function oci_pconnect($username, $password, $db, $charset, $session_mode);
/**
* Returns field's value from the fetched row
* @return mixed
* @version PHP 5
* @param $statement resource
* @param $field mixed
*/
function oci_result($statement, $field);
/**
* Rolls back outstanding transaction
* @return bool
* @version PHP 5
* @param $connection resource
*/
function oci_rollback($connection);
/**
* Returns server version
* @return string
* @version PHP 5
* @param $connection resource
*/
function oci_server_version($connection);
/**
* Sets number of rows to be prefetched
* @return bool
* @version PHP 5
* @param $statement resource
* @param $rows (optional) int
*/
function oci_set_prefetch($statement, $rows);
/**
* Returns the type of an OCI statement
* @return string
* @version PHP 5
* @param $statement resource
*/
function oci_statement_type($statement);
/**
* Octal to decimal
* @return number
* @version PHP 3, PHP 4, PHP 5
* @param $octal_string string
*/
function octdec($octal_string);
/**
* Toggle autocommit behaviour
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
* @param $OnOff (optional) bool
*/
function odbc_autocommit($connection_id, $OnOff);
/**
* Handling of binary column data
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $mode int
*/
function odbc_binmode($result_id, $mode);
/**
* Close an ODBC connection
* @return 
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
*/
function odbc_close($connection_id);
/**
* Close all ODBC connections
* @return 
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
*/
function odbc_close_all();
/**
* Returns a result identifier that can be used to fetch a list of columns and associated privileges
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier string
* @param $owner string
* @param $table_name string
* @param $column_name string
*/
function odbc_columnprivileges($connection_id, $qualifier, $owner, $table_name, $column_name);
/**
* Lists the column names in specified tables
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier (optional) string
* @param $schema (optional) string
* @param $table_name (optional) string
* @param $column_name (optional) string
*/
function odbc_columns($connection_id, $qualifier, $schema, $table_name, $column_name);
/**
* Commit an ODBC transaction
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
*/
function odbc_commit($connection_id);
/**
* Connect to a datasource
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $dsn string
* @param $user string
* @param $password string
* @param $cursor_type (optional) int
*/
function odbc_connect($dsn, $user, $password, $cursor_type);
/**
* Get cursorname
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
*/
function odbc_cursor($result_id);
/**
* Returns information about a current connection
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection_id resource
* @param $fetch_type int
*/
function odbc_data_source($connection_id, $fetch_type);
/**
* Synonym for odbc_exec()
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $conn_id resource
* @param $query string
*/
function odbc_do($conn_id, $query);
/**
* Get the last error code
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $connection_id resource
*/
function odbc_error($connection_id);
/**
* Get the last error message
* @return string
* @version PHP 4 >= 4.0.5, PHP 5
* @param $connection_id resource
*/
function odbc_errormsg($connection_id);
/**
* Prepare and execute a SQL statement
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
* @param $query_string string
* @param $flags (optional) int
*/
function odbc_exec($connection_id, $query_string, $flags);
/**
* Execute a prepared statement
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $parameters_array (optional) array
*/
function odbc_execute($result_id, $parameters_array);
/**
* Fetch a result row as an associative array
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $result resource
* @param $rownumber (optional) int
*/
function odbc_fetch_array($result, $rownumber);
/**
* Fetch one result row into array
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param &$result_array array
* @param $rownumber (optional) int
*/
function odbc_fetch_into($result_id, &$result_array, $rownumber);
/**
* Fetch a result row as an object
* @return object
* @version PHP 4 >= 4.0.2, PHP 5
* @param $result resource
* @param $rownumber (optional) int
*/
function odbc_fetch_object($result, $rownumber);
/**
* Fetch a row
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $row_number (optional) int
*/
function odbc_fetch_row($result_id, $row_number);
/**
* Get the length (precision) of a field
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $field_number int
*/
function odbc_field_len($result_id, $field_number);
/**
* Get the columnname
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $field_number int
*/
function odbc_field_name($result_id, $field_number);
/**
* Return column number
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $field_name string
*/
function odbc_field_num($result_id, $field_name);
/**
* Synonym for odbc_field_len()
* @return int
* @version PHP 4, PHP 5
* @param $result_id resource
* @param $field_number int
*/
function odbc_field_precision($result_id, $field_number);
/**
* Get the scale of a field
* @return int
* @version PHP 4, PHP 5
* @param $result_id resource
* @param $field_number int
*/
function odbc_field_scale($result_id, $field_number);
/**
* Datatype of a field
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $field_number int
*/
function odbc_field_type($result_id, $field_number);
/**
* Returns a list of foreign keys in the specified table or a list of foreign keys in other tables that refer to the primary key in the specified table
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $pk_qualifier string
* @param $pk_owner string
* @param $pk_table string
* @param $fk_qualifier string
* @param $fk_owner string
* @param $fk_table string
*/
function odbc_foreignkeys($connection_id, $pk_qualifier, $pk_owner, $pk_table, $fk_qualifier, $fk_owner, $fk_table);
/**
* Free resources associated with a result
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
*/
function odbc_free_result($result_id);
/**
* Returns a result identifier containing information about data types supported by the data source
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $data_type (optional) int
*/
function odbc_gettypeinfo($connection_id, $data_type);
/**
* Handling of LONG columns
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $length int
*/
function odbc_longreadlen($result_id, $length);
/**
* Checks if multiple results are available
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $result_id resource
*/
function odbc_next_result($result_id);
/**
* Number of columns in a result
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
*/
function odbc_num_fields($result_id);
/**
* Number of rows in a result
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
*/
function odbc_num_rows($result_id);
/**
* Open a persistent database connection
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $dsn string
* @param $user string
* @param $password string
* @param $cursor_type (optional) int
*/
function odbc_pconnect($dsn, $user, $password, $cursor_type);
/**
* Prepares a statement for execution
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
* @param $query_string string
*/
function odbc_prepare($connection_id, $query_string);
/**
* Returns a result identifier that can be used to fetch the column names that comprise the primary key for a table
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier string
* @param $owner string
* @param $table string
*/
function odbc_primarykeys($connection_id, $qualifier, $owner, $table);
/**
* Retrieve information about parameters to procedures
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier (optional) string
* @param $owner (optional) string
* @param $proc (optional) string
* @param $column (optional) string
*/
function odbc_procedurecolumns($connection_id, $qualifier, $owner, $proc, $column);
/**
* Get the list of procedures stored in a specific data source
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier (optional) string
* @param $owner (optional) string
* @param $name (optional) string
*/
function odbc_procedures($connection_id, $qualifier, $owner, $name);
/**
* Get result data
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $field mixed
*/
function odbc_result($result_id, $field);
/**
* Print result as HTML table
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $result_id resource
* @param $format (optional) string
*/
function odbc_result_all($result_id, $format);
/**
* Rollback a transaction
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $connection_id resource
*/
function odbc_rollback($connection_id);
/**
* Adjust ODBC settings
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $id resource
* @param $function int
* @param $option int
* @param $param int
*/
function odbc_setoption($id, $function, $option, $param);
/**
* Returns either the optimal set of columns that uniquely identifies a row in the table or columns that are automatically updated when any value in the row is updated by a transaction
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $type int
* @param $qualifier string
* @param $owner string
* @param $table string
* @param $scope int
* @param $nullable int
*/
function odbc_specialcolumns($connection_id, $type, $qualifier, $owner, $table, $scope, $nullable);
/**
* Retrieve statistics about a table
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier string
* @param $owner string
* @param $table_name string
* @param $unique int
* @param $accuracy int
*/
function odbc_statistics($connection_id, $qualifier, $owner, $table_name, $unique, $accuracy);
/**
* Lists tables and the privileges associated with each table
* @return resource
* @version PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier string
* @param $owner string
* @param $name string
*/
function odbc_tableprivileges($connection_id, $qualifier, $owner, $name);
/**
* Get the list of table names stored in a specific data source
* @return resource
* @version PHP 3 >= 3.0.17, PHP 4, PHP 5
* @param $connection_id resource
* @param $qualifier (optional) string
* @param $owner (optional) string
* @param $name (optional) string
* @param $types (optional) string
*/
function odbc_tables($connection_id, $qualifier, $owner, $name, $types);
/**
* Generate OpenAL buffer
* @return resource
* @version PECL
*/
function openal_buffer_create();
/**
* Load a buffer with data
* @return bool
* @version PECL
* @param $buffer resource
* @param $format int
* @param $data string
* @param $freq int
*/
function openal_buffer_data($buffer, $format, $data, $freq);
/**
* Destroys an OpenAL buffer
* @return bool
* @version PECL
* @param $buffer resource
*/
function openal_buffer_destroy($buffer);
/**
* Retrieve an OpenAL buffer property
* @return int
* @version PECL
* @param $buffer resource
* @param $property int
*/
function openal_buffer_get($buffer, $property);
/**
* Load a .wav file into a buffer
* @return bool
* @version PECL
* @param $buffer resource
* @param $wavfile string
*/
function openal_buffer_loadwav($buffer, $wavfile);
/**
* Create an audio processing context
* @return resource
* @version PECL
* @param $device resource
*/
function openal_context_create($device);
/**
* Make the specified context current
* @return bool
* @version PECL
* @param $context resource
*/
function openal_context_current($context);
/**
* Destroys a context
* @return bool
* @version PECL
* @param $context resource
*/
function openal_context_destroy($context);
/**
* Process the specified context
* @return bool
* @version PECL
* @param $context resource
*/
function openal_context_process($context);
/**
* Suspend the specified context
* @return bool
* @version PECL
* @param $context resource
*/
function openal_context_suspend($context);
/**
* Close an OpenAL device
* @return bool
* @version PECL
* @param $device resource
*/
function openal_device_close($device);
/**
* Initialize the OpenAL audio layer
* @return resource
* @version PECL
* @param $device_desc string
*/
function openal_device_open($device_desc);
/**
* Retrieve a listener property
* @return mixed
* @version PECL
* @param $property int
*/
function openal_listener_get($property);
/**
* Set a listener property
* @return bool
* @version PECL
* @param $property int
* @param $setting mixed
*/
function openal_listener_set($property, $setting);
/**
* Generate a source resource
* @return resource
* @version PECL
*/
function openal_source_create();
/**
* Destroy a source resource
* @return bool
* @version PECL
* @param $source resource
*/
function openal_source_destroy($source);
/**
* Retrieve an OpenAL source property
* @return mixed
* @version PECL
* @param $source resource
* @param $property int
*/
function openal_source_get($source, $property);
/**
* Pause the source
* @return bool
* @version PECL
* @param $source resource
*/
function openal_source_pause($source);
/**
* Start playing the source
* @return bool
* @version PECL
* @param $source resource
*/
function openal_source_play($source);
/**
* Rewind the source
* @return bool
* @version PECL
* @param $source resource
*/
function openal_source_rewind($source);
/**
* Set source property
* @return bool
* @version PECL
* @param $source resource
* @param $property int
* @param $setting mixed
*/
function openal_source_set($source, $property, $setting);
/**
* Stop playing the source
* @return bool
* @version PECL
* @param $source resource
*/
function openal_source_stop($source);
/**
* Begin streaming on a source
* @return resource
* @version PECL
* @param $source resource
* @param $format int
* @param $rate int
*/
function openal_stream($source, $format, $rate);
/**
* Open directory handle
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $path string
* @param $context (optional) resource
*/
function opendir($path, $context);
/**
* Open connection to system logger
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $ident string
* @param $option int
* @param $facility int
*/
function openlog($ident, $option, $facility);
/**
* Exports a CSR as a string
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $csr resource
* @param &$out string
* @param $notext (optional) bool
*/
function openssl_csr_export($csr, &$out, $notext);
/**
* Exports a CSR to a file
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $csr resource
* @param $outfilename string
* @param $notext (optional) bool
*/
function openssl_csr_export_to_file($csr, $outfilename, $notext);
/**
* Generates a CSR
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5
* @param $dn array
* @param &$privkey resource
* @param $configargs (optional) array
* @param $extraattribs (optional) array
*/
function openssl_csr_new($dn, &$privkey, $configargs, $extraattribs);
/**
* Sign a CSR with another certificate (or itself) and generate a certificate
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $csr mixed
* @param $cacert mixed
* @param $priv_key mixed
* @param $days int
* @param $configargs (optional) array
* @param $serial (optional) int
*/
function openssl_csr_sign($csr, $cacert, $priv_key, $days, $configargs, $serial);
/**
* Return openSSL error message
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
*/
function openssl_error_string();
/**
* Free key resource
* @return 
* @version PHP 4 >= 4.0.4, PHP 5
* @param $key_identifier resource
*/
function openssl_free_key($key_identifier);
/**
* Alias of openssl_pkey_get_private()
* @return &#13;
* @version PHP 4 >= 4.0.4, PHP 5
*/
function openssl_get_privatekey();
/**
* Alias of openssl_pkey_get_public()
* @return &#13;
* @version PHP 4 >= 4.0.4, PHP 5
*/
function openssl_get_publickey();
/**
* Open sealed data
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $sealed_data string
* @param &$open_data string
* @param $env_key string
* @param $priv_key_id mixed
*/
function openssl_open($sealed_data, &$open_data, $env_key, $priv_key_id);
/**
* Decrypts an S/MIME encrypted message
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $infilename string
* @param $outfilename string
* @param $recipcert mixed
* @param $recipkey (optional) mixed
*/
function openssl_pkcs7_decrypt($infilename, $outfilename, $recipcert, $recipkey);
/**
* Encrypt an S/MIME message
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $infile string
* @param $outfile string
* @param $recipcerts mixed
* @param $headers array
* @param $flags (optional) int
* @param $cipherid (optional) int
*/
function openssl_pkcs7_encrypt($infile, $outfile, $recipcerts, $headers, $flags, $cipherid);
/**
* Sign an S/MIME message
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $infilename string
* @param $outfilename string
* @param $signcert mixed
* @param $privkey mixed
* @param $headers array
* @param $flags (optional) int
* @param $extracerts (optional) string
*/
function openssl_pkcs7_sign($infilename, $outfilename, $signcert, $privkey, $headers, $flags, $extracerts);
/**
* Verifies the signature of an S/MIME signed message
* @return mixed
* @version PHP 4 >= 4.0.6, PHP 5
* @param $filename string
* @param $flags int
* @param $outfilename (optional) string
* @param $cainfo (optional) array
* @param $extracerts (optional) string
*/
function openssl_pkcs7_verify($filename, $flags, $outfilename, $cainfo, $extracerts);
/**
* Gets an exportable representation of a key into a string
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $key mixed
* @param &$out string
* @param $passphrase (optional) string
* @param $configargs (optional) array
*/
function openssl_pkey_export($key, &$out, $passphrase, $configargs);
/**
* Gets an exportable representation of a key into a file
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $key mixed
* @param $outfilename string
* @param $passphrase (optional) string
* @param $configargs (optional) array
*/
function openssl_pkey_export_to_file($key, $outfilename, $passphrase, $configargs);
/**
* Frees a private key
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $key resource
*/
function openssl_pkey_free($key);
/**
* Get a private key
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $key mixed
* @param $passphrase (optional) string
*/
function openssl_pkey_get_private($key, $passphrase);
/**
* Extract public key from certificate and prepare it for use
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $certificate mixed
*/
function openssl_pkey_get_public($certificate);
/**
* Generates a new private key
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $configargs array
*/
function openssl_pkey_new($configargs);
/**
* Decrypts data with private key
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $data string
* @param &$decrypted string
* @param $key mixed
* @param $padding (optional) int
*/
function openssl_private_decrypt($data, &$decrypted, $key, $padding);
/**
* Encrypts data with private key
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $data string
* @param &$crypted string
* @param $key mixed
* @param $padding (optional) int
*/
function openssl_private_encrypt($data, &$crypted, $key, $padding);
/**
* Decrypts data with public key
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $data string
* @param &$decrypted string
* @param $key mixed
* @param $padding (optional) int
*/
function openssl_public_decrypt($data, &$decrypted, $key, $padding);
/**
* Encrypts data with public key
* @return bool
* @version PHP 4 >= 4.0.6, PHP 5
* @param $data string
* @param &$crypted string
* @param $key mixed
* @param $padding (optional) int
*/
function openssl_public_encrypt($data, &$crypted, $key, $padding);
/**
* Seal (encrypt) data
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param &$sealed_data string
* @param &$env_keys array
* @param $pub_key_ids array
*/
function openssl_seal($data, &$sealed_data, &$env_keys, $pub_key_ids);
/**
* Generate signature
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param &$signature string
* @param $priv_key_id mixed
* @param $signature_alg (optional) int
*/
function openssl_sign($data, &$signature, $priv_key_id, $signature_alg);
/**
* Verify signature
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $data string
* @param $signature string
* @param $pub_key_id mixed
*/
function openssl_verify($data, $signature, $pub_key_id);
/**
* Verifies if a certificate can be used for a particular purpose
* @return int
* @version PHP 4 >= 4.0.6, PHP 5
* @param $x509cert mixed
* @param $purpose int
* @param $cainfo (optional) array
* @param $untrustedfile (optional) string
*/
function openssl_x509_checkpurpose($x509cert, $purpose, $cainfo, $untrustedfile);
/**
* Checks if a private key corresponds to a certificate
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $cert mixed
* @param $key mixed
*/
function openssl_x509_check_private_key($cert, $key);
/**
* Exports a certificate as a string
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $x509 mixed
* @param &$output string
* @param $notext (optional) bool
*/
function openssl_x509_export($x509, &$output, $notext);
/**
* Exports a certificate to file
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $x509 mixed
* @param $outfilename string
* @param $notext (optional) bool
*/
function openssl_x509_export_to_file($x509, $outfilename, $notext);
/**
* Free certificate resource
* @return 
* @version PHP 4 >= 4.0.6, PHP 5
* @param $x509cert resource
*/
function openssl_x509_free($x509cert);
/**
* Parse an X509 certificate and return the information as an array
* @return array
* @version PHP 4 >= 4.0.6, PHP 5
* @param $x509cert mixed
* @param $shortnames (optional) bool
*/
function openssl_x509_parse($x509cert, $shortnames);
/**
* Parse an X.509 certificate and return a resource identifier for it
* @return resource
* @version PHP 4 >= 4.0.6, PHP 5
* @param $x509certdata mixed
*/
function openssl_x509_read($x509certdata);
/**
* Binds a PHP variable to an Oracle parameter
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $PHP_variable_name string
* @param $SQL_parameter_name string
* @param $length int
* @param $type (optional) int
*/
function ora_bind($cursor, $PHP_variable_name, $SQL_parameter_name, $length, $type);
/**
* Closes an Oracle cursor
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
*/
function ora_close($cursor);
/**
* Gets the name of an Oracle result column
* @return string
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $column int
*/
function ora_columnname($cursor, $column);
/**
* Returns the size of an Oracle result column
* @return int
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $column int
*/
function ora_columnsize($cursor, $column);
/**
* Gets the type of an Oracle result column
* @return string
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $column int
*/
function ora_columntype($cursor, $column);
/**
* Commit an Oracle transaction
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $conn resource
*/
function ora_commit($conn);
/**
* Disable automatic commit
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $conn resource
*/
function ora_commitoff($conn);
/**
* Enable automatic commit
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $conn resource
*/
function ora_commiton($conn);
/**
* Parse, Exec, Fetch
* @return resource
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $conn resource
* @param $query string
*/
function ora_do($conn, $query);
/**
* Gets an Oracle error message
* @return string
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor_or_connection resource
*/
function ora_error($cursor_or_connection);
/**
* Gets an Oracle error code
* @return int
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor_or_connection resource
*/
function ora_errorcode($cursor_or_connection);
/**
* Execute a parsed statement on an Oracle cursor
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
*/
function ora_exec($cursor);
/**
* Fetch a row of data from a cursor
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
*/
function ora_fetch($cursor);
/**
* Fetch a row into the specified result array
* @return int
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param &$result array
* @param $flags (optional) int
*/
function ora_fetch_into($cursor, &$result, $flags);
/**
* Get data from a fetched column
* @return string
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $column int
*/
function ora_getcolumn($cursor, $column);
/**
* Close an Oracle connection
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $connection resource
*/
function ora_logoff($connection);
/**
* Open an Oracle connection
* @return resource
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $user string
* @param $password string
*/
function ora_logon($user, $password);
/**
* Returns the number of columns
* @return int
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
*/
function ora_numcols($cursor);
/**
* Returns the number of rows
* @return int
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
*/
function ora_numrows($cursor);
/**
* Opens an Oracle cursor
* @return resource
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $connection resource
*/
function ora_open($connection);
/**
* Parse an SQL statement with Oracle
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $cursor resource
* @param $sql_statement string
* @param $defer (optional) int
*/
function ora_parse($cursor, $sql_statement, $defer);
/**
* Open a persistent Oracle connection
* @return resource
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $user string
* @param $password string
*/
function ora_plogon($user, $password);
/**
* Rolls back a transaction
* @return bool
* @version PHP 3, PHP 4, PHP 5 <= 5.1.0RC1
* @param $connection resource
*/
function ora_rollback($connection);
/**
* Return ASCII value of character
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function ord($string);
/**
* Add URL rewriter values
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $name string
* @param $value string
*/
function output_add_rewrite_var($name, $value);
/**
* Reset URL rewriter values
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
*/
function output_reset_rewrite_vars();
/**
* Enable property and method call overloading for a class
* @return 
* @version PHP 4 >= 4.2.0
* @param $class_name string
*/
function overload($class_name);
/**
* Overrides built-in functions
* @return bool
* @version PECL
* @param $function_name string
* @param $function_args string
* @param $function_code string
*/
function override_function($function_name, $function_args, $function_code);
/**
* Closes the connection to ovrimos
* @return 
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection int
*/
function ovrimos_close($connection);
/**
* Commits the transaction
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection_id int
*/
function ovrimos_commit($connection_id);
/**
* Connect to the specified database
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $host string
* @param $db string
* @param $user string
* @param $password string
*/
function ovrimos_connect($host, $db, $user, $password);
/**
* Returns the name of the cursor
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
*/
function ovrimos_cursor($result_id);
/**
* Executes an SQL statement
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection_id int
* @param $query string
*/
function ovrimos_exec($connection_id, $query);
/**
* Executes a prepared SQL statement
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $parameters_array (optional) array
*/
function ovrimos_execute($result_id, $parameters_array);
/**
* Fetches a row from the result set
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param &$result_array array
* @param $how (optional) string
* @param $rownumber (optional) int
*/
function ovrimos_fetch_into($result_id, &$result_array, $how, $rownumber);
/**
* Fetches a row from the result set
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $how (optional) int
* @param $row_number (optional) int
*/
function ovrimos_fetch_row($result_id, $how, $row_number);
/**
* Returns the length of the output column
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $field_number int
*/
function ovrimos_field_len($result_id, $field_number);
/**
* Returns the output column name
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $field_number int
*/
function ovrimos_field_name($result_id, $field_number);
/**
* Returns the (1-based) index of the output column
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $field_name string
*/
function ovrimos_field_num($result_id, $field_name);
/**
* Returns the (numeric) type of the output column
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $field_number int
*/
function ovrimos_field_type($result_id, $field_number);
/**
* Frees the specified result_id
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
*/
function ovrimos_free_result($result_id);
/**
* Specifies how many bytes are to be retrieved from long datatypes
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $length int
*/
function ovrimos_longreadlen($result_id, $length);
/**
* Returns the number of columns
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
*/
function ovrimos_num_fields($result_id);
/**
* Returns the number of rows affected by update operations
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
*/
function ovrimos_num_rows($result_id);
/**
* Prepares an SQL statement
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection_id int
* @param $query string
*/
function ovrimos_prepare($connection_id, $query);
/**
* Retrieves the output column
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $field mixed
*/
function ovrimos_result($result_id, $field);
/**
* Prints the whole result set as an HTML table
* @return int
* @version PHP 4 >= 4.0.3, PHP 5
* @param $result_id int
* @param $format (optional) string
*/
function ovrimos_result_all($result_id, $format);
/**
* Rolls back the transaction
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection_id int
*/
function ovrimos_rollback($connection_id);
/**
* Pack data into binary string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function pack($format, $args, $params1);
/**
* Compile a string of PHP code and return the resulting op array
* @return array
* @version PECL
* @param $filename string
* @param &$errors (optional) array
* @param $options (optional) int
*/
function parsekit_compile_file($filename, &$errors, $options);
/**
* Compile a string of PHP code and return the resulting op array
* @return array
* @version PECL
* @param $phpcode string
* @param &$errors (optional) array
* @param $options (optional) int
*/
function parsekit_compile_string($phpcode, &$errors, $options);
/**
* Return information regarding function argument(s)
* @return array
* @version PECL
* @param $function mixed
*/
function parsekit_func_arginfo($function);
/**
* Parse a configuration file
* @return array
* @version PHP 4, PHP 5
* @param $filename string
* @param $process_sections (optional) bool
*/
function parse_ini_file($filename, $process_sections);
/**
* Parses the string into variables
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param &$arr (optional) array
*/
function parse_str($str, &$arr);
/**
* Parse a URL and return its components
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $url string
*/
function parse_url($url);
/**
* Execute an external program and display raw output
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $command string
* @param &$return_var (optional) int
*/
function passthru($command, &$return_var);
/**
* Returns information about a file path
* @return mixed
* @version PHP 4 >= 4.0.3, PHP 5
* @param $path string
* @param $options (optional) int
*/
function pathinfo($path, $options);
/**
* Closes process file pointer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function pclose($handle);
/**
* Set an alarm clock for delivery of a signal
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $seconds int
*/
function pcntl_alarm($seconds);
/**
* Executes specified program in current process space
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $path string
* @param $args (optional) array
* @param $envs (optional) array
*/
function pcntl_exec($path, $args, $envs);
/**
* Forks the currently running process
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
*/
function pcntl_fork();
/**
* Get the priority of any process
* @return int
* @version PHP 5
* @param $pid int
* @param $process_identifier (optional) int
*/
function pcntl_getpriority($pid, $process_identifier);
/**
* Change the priority of any process
* @return bool
* @version PHP 5
* @param $priority int
* @param $pid (optional) int
* @param $process_identifier (optional) int
*/
function pcntl_setpriority($priority, $pid, $process_identifier);
/**
* Installs a signal handler
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $signo int
* @param $handle callback
* @param $restart_syscalls (optional) bool
*/
function pcntl_signal($signo, $handle, $restart_syscalls);
/**
* Waits on or returns the status of a forked child
* @return int
* @version PHP 5
* @param &$status int
* @param $options (optional) int
*/
function pcntl_wait(&$status, $options);
/**
* Waits on or returns the status of a forked child
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $pid int
* @param &$status int
* @param $options (optional) int
*/
function pcntl_waitpid($pid, &$status, $options);
/**
* Returns the return code of a terminated child
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wexitstatus($status);
/**
* Returns TRUE if status code represents a successful exit
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wifexited($status);
/**
* Returns TRUE if status code represents a termination due to a signal
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wifsignaled($status);
/**
* Returns TRUE if child process is currently stopped
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wifstopped($status);
/**
* Returns the signal which caused the child to stop
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wstopsig($status);
/**
* Returns the signal which caused the child to terminate
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $status int
*/
function pcntl_wtermsig($status);
/**
* Activate structure element or other content item
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $id int
*/
function PDF_activate_item($pdfdoc, $id);
/**
* Add annotation [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.12, PHP 4, PECL
*/
function PDF_add_annotation();
/**
* Add bookmark for current page [deprecated]
* @return &#13;
* @version PHP 4 >= 4.0.1, PECL
*/
function PDF_add_bookmark();
/**
* Add launch annotation for current page [deprecated]
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $filename string
*/
function PDF_add_launchlink($pdfdoc, $llx, $lly, $urx, $ury, $filename);
/**
* Add link annotation for current page [deprecated]
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $lowerleftx float
* @param $lowerlefty float
* @param $upperrightx float
* @param $upperrighty float
* @param $page int
* @param $dest string
*/
function PDF_add_locallink($pdfdoc, $lowerleftx, $lowerlefty, $upperrightx, $upperrighty, $page, $dest);
/**
* Create named destination
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $name string
* @param $optlist string
*/
function PDF_add_nameddest($pdfdoc, $name, $optlist);
/**
* Set annotation for current page [deprecated]
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $contents string
* @param $title string
* @param $icon string
* @param $open int
*/
function PDF_add_note($pdfdoc, $llx, $lly, $urx, $ury, $contents, $title, $icon, $open);
/**
* Add bookmark for current page [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_add_outline();
/**
* Add file link annotation for current page [deprecated]
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PECL
* @param $pdfdoc resource
* @param $bottom_left_x float
* @param $bottom_left_y float
* @param $up_right_x float
* @param $up_right_y float
* @param $filename string
* @param $page int
* @param $dest string
*/
function PDF_add_pdflink($pdfdoc, $bottom_left_x, $bottom_left_y, $up_right_x, $up_right_y, $filename, $page, $dest);
/**
* Add thumbnail for current page
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $image int
*/
function PDF_add_thumbnail($pdfdoc, $image);
/**
* Add weblink for current page [deprecated]
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PECL
* @param $pdfdoc resource
* @param $lowerleftx float
* @param $lowerlefty float
* @param $upperrightx float
* @param $upperrighty float
* @param $url string
*/
function PDF_add_weblink($pdfdoc, $lowerleftx, $lowerlefty, $upperrightx, $upperrighty, $url);
/**
* Draw a counterclockwise circular arc segment
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x float
* @param $y float
* @param $r float
* @param $alpha float
* @param $beta float
*/
function PDF_arc($p, $x, $y, $r, $alpha, $beta);
/**
* Draw a clockwise circular arc segment
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $x float
* @param $y float
* @param $r float
* @param $alpha float
* @param $beta float
*/
function PDF_arcn($p, $x, $y, $r, $alpha, $beta);
/**
* Add file attachment for current page [deprecated]
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $filename string
* @param $description string
* @param $author string
* @param $mimetype string
* @param $icon string
*/
function PDF_attach_file($pdfdoc, $llx, $lly, $urx, $ury, $filename, $description, $author, $mimetype, $icon);
/**
* Create new PDF file
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $filename string
* @param $optlist string
*/
function PDF_begin_document($pdfdoc, $filename, $optlist);
/**
* Start a Type 3 font definition
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $filename string
* @param $a float
* @param $b float
* @param $c float
* @param $d float
* @param $e float
* @param $f float
* @param $optlist string
*/
function PDF_begin_font($pdfdoc, $filename, $a, $b, $c, $d, $e, $f, $optlist);
/**
* Start glyph definition for Type 3 font
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $glyphname string
* @param $wx float
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
*/
function PDF_begin_glyph($pdfdoc, $glyphname, $wx, $llx, $lly, $urx, $ury);
/**
* Open structure element or other content item
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $tag string
* @param $optlist string
*/
function PDF_begin_item($pdfdoc, $tag, $optlist);
/**
* Start layer
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $layer int
*/
function PDF_begin_layer($pdfdoc, $layer);
/**
* Start new page [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $width float
* @param $height float
*/
function PDF_begin_page($pdfdoc, $width, $height);
/**
* Start new page
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $width float
* @param $height float
* @param $optlist string
*/
function PDF_begin_page_ext($pdfdoc, $width, $height, $optlist);
/**
* Start pattern definition
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $width float
* @param $height float
* @param $xstep float
* @param $ystep float
* @param $painttype int
*/
function PDF_begin_pattern($pdfdoc, $width, $height, $xstep, $ystep, $painttype);
/**
* Start template definition
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $width float
* @param $height float
*/
function PDF_begin_template($pdfdoc, $width, $height);
/**
* Draw a circle
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $x float
* @param $y float
* @param $r float
*/
function PDF_circle($pdfdoc, $x, $y, $r);
/**
* Clip to current path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_clip($p);
/**
* Close pdf resource [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_close($p);
/**
* Close current path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_closepath($p);
/**
* Close, fill and stroke current path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_closepath_fill_stroke($p);
/**
* Close and stroke path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_closepath_stroke($p);
/**
* Close image
* @return 
* @version PHP 3 >= 3.0.7, PHP 4, PECL
* @param $p resource
* @param $image int
*/
function PDF_close_image($p, $image);
/**
* Close the input PDF document
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $doc int
*/
function PDF_close_pdi($p, $doc);
/**
* Close the page handle
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $page int
*/
function PDF_close_pdi_page($p, $page);
/**
* Concatenate a matrix to the CTM
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $a float
* @param $b float
* @param $c float
* @param $d float
* @param $e float
* @param $f float
*/
function PDF_concat($p, $a, $b, $c, $d, $e, $f);
/**
* Output text in next line
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $text string
*/
function PDF_continue_text($p, $text);
/**
* Create action for objects or events
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $type string
* @param $optlist string
*/
function PDF_create_action($pdfdoc, $type, $optlist);
/**
* Create rectangular annotation
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $type string
* @param $optlist string
*/
function PDF_create_annotation($pdfdoc, $llx, $lly, $urx, $ury, $type, $optlist);
/**
* Create bookmark
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $text string
* @param $optlist string
*/
function PDF_create_bookmark($pdfdoc, $text, $optlist);
/**
* Create form field
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $name string
* @param $type string
* @param $optlist string
*/
function PDF_create_field($pdfdoc, $llx, $lly, $urx, $ury, $name, $type, $optlist);
/**
* Create form field group
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $name string
* @param $optlist string
*/
function PDF_create_fieldgroup($pdfdoc, $name, $optlist);
/**
* Create graphics state object
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_create_gstate($pdfdoc, $optlist);
/**
* Create PDFlib virtual file
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $filename string
* @param $data string
* @param $optlist string
*/
function PDF_create_pvf($pdfdoc, $filename, $data, $optlist);
/**
* Create textflow object
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $text string
* @param $optlist string
*/
function PDF_create_textflow($pdfdoc, $text, $optlist);
/**
* Draw Bezier curve
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $x3 float
* @param $y3 float
*/
function PDF_curveto($p, $x1, $y1, $x2, $y2, $x3, $y3);
/**
* Create layer definition
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $name string
* @param $optlist string
*/
function PDF_define_layer($pdfdoc, $name, $optlist);
/**
* Delete PDFlib object
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
*/
function PDF_delete($pdfdoc);
/**
* Delete PDFlib virtual file
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $filename string
*/
function PDF_delete_pvf($pdfdoc, $filename);
/**
* Delete textflow object
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $textflow int
*/
function PDF_delete_textflow($pdfdoc, $textflow);
/**
* Add glyph name and/or Unicode value
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $encoding string
* @param $slot int
* @param $glyphname string
* @param $uv int
*/
function PDF_encoding_set_char($pdfdoc, $encoding, $slot, $glyphname, $uv);
/**
* Close PDF file
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_end_document($pdfdoc, $optlist);
/**
* Terminate Type 3 font definition
* @return bool
* @version PECL
* @param $pdfdoc resource
*/
function PDF_end_font($pdfdoc);
/**
* Terminate glyph definition for Type 3 font
* @return bool
* @version PECL
* @param $pdfdoc resource
*/
function PDF_end_glyph($pdfdoc);
/**
* Close structure element or other content item
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $id int
*/
function PDF_end_item($pdfdoc, $id);
/**
* Deactivate all active layers
* @return bool
* @version PECL
* @param $pdfdoc resource
*/
function PDF_end_layer($pdfdoc);
/**
* Finish page
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_end_page($p);
/**
* Finish page
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_end_page_ext($pdfdoc, $optlist);
/**
* Finish pattern
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
*/
function PDF_end_pattern($p);
/**
* Finish template
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
*/
function PDF_end_template($p);
/**
* Fill current path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_fill($p);
/**
* Fill image block with variable data
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $page int
* @param $blockname string
* @param $image int
* @param $optlist string
*/
function PDF_fill_imageblock($pdfdoc, $page, $blockname, $image, $optlist);
/**
* Fill PDF block with variable data
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $page int
* @param $blockname string
* @param $contents int
* @param $optlist string
*/
function PDF_fill_pdfblock($pdfdoc, $page, $blockname, $contents, $optlist);
/**
* Fill and stroke path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_fill_stroke($p);
/**
* Fill text block with variable data
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $page int
* @param $blockname string
* @param $text string
* @param $optlist string
*/
function PDF_fill_textblock($pdfdoc, $page, $blockname, $text, $optlist);
/**
* Prepare font for later use [deprecated]
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $fontname string
* @param $encoding string
* @param $embed int
*/
function PDF_findfont($p, $fontname, $encoding, $embed);
/**
* Place image or template
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $image int
* @param $x float
* @param $y float
* @param $optlist string
*/
function PDF_fit_image($pdfdoc, $image, $x, $y, $optlist);
/**
* Place imported PDF page
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $page int
* @param $x float
* @param $y float
* @param $optlist string
*/
function PDF_fit_pdi_page($pdfdoc, $page, $x, $y, $optlist);
/**
* Format textflow in rectangular area
* @return string
* @version PECL
* @param $pdfdoc resource
* @param $textflow int
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $optlist string
*/
function PDF_fit_textflow($pdfdoc, $textflow, $llx, $lly, $urx, $ury, $optlist);
/**
* Place single line of text
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $text string
* @param $x float
* @param $y float
* @param $optlist string
*/
function PDF_fit_textline($pdfdoc, $text, $x, $y, $optlist);
/**
* Get name of unsuccessfull API function
* @return string
* @version PECL
* @param $pdfdoc resource
*/
function PDF_get_apiname($pdfdoc);
/**
* Get PDF output buffer
* @return string
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
*/
function PDF_get_buffer($p);
/**
* Get error text
* @return string
* @version PECL
* @param $pdfdoc resource
*/
function PDF_get_errmsg($pdfdoc);
/**
* Get error number
* @return int
* @version PECL
* @param $pdfdoc resource
*/
function PDF_get_errnum($pdfdoc);
/**
* Get font [deprecated]
* @return &#13;
* @version PHP 4, PECL
*/
function PDF_get_font();
/**
* Get font name [deprecated]
* @return &#13;
* @version PHP 4, PECL
*/
function PDF_get_fontname();
/**
* Font handling [deprecated]
* @return &#13;
* @version PHP 4, PECL
*/
function PDF_get_fontsize();
/**
* Get image height [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.12, PHP 4, PECL
*/
function PDF_get_image_height();
/**
* Get image width [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.12, PHP 4, PECL
*/
function PDF_get_image_width();
/**
* Get major version number [deprecated]
* @return int
* @version PHP 4 >= 4.2.0, PECL
*/
function PDF_get_majorversion();
/**
* Get minor version number [deprecated]
* @return int
* @version PHP 4 >= 4.2.0, PECL
*/
function PDF_get_minorversion();
/**
* Get string parameter
* @return string
* @version PHP 4 >= 4.0.1, PECL
* @param $p resource
* @param $key string
* @param $modifier float
*/
function PDF_get_parameter($p, $key, $modifier);
/**
* Get PDI string parameter
* @return string
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $key string
* @param $doc int
* @param $page int
* @param $reserved int
*/
function PDF_get_pdi_parameter($p, $key, $doc, $page, $reserved);
/**
* Get PDI numerical parameter
* @return float
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $key string
* @param $doc int
* @param $page int
* @param $reserved int
*/
function PDF_get_pdi_value($p, $key, $doc, $page, $reserved);
/**
* Get numerical parameter
* @return float
* @version PHP 4 >= 4.0.1, PECL
* @param $p resource
* @param $key string
* @param $modifier float
*/
function PDF_get_value($p, $key, $modifier);
/**
* Query textflow state
* @return float
* @version PECL
* @param $pdfdoc resource
* @param $textflow int
* @param $keyword string
*/
function PDF_info_textflow($pdfdoc, $textflow, $keyword);
/**
* Reset graphic state
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
*/
function PDF_initgraphics($p);
/**
* Draw a line
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x float
* @param $y float
*/
function PDF_lineto($p, $x, $y);
/**
* Search and prepare font
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $fontname string
* @param $encoding string
* @param $optlist string
*/
function PDF_load_font($pdfdoc, $fontname, $encoding, $optlist);
/**
* Search and prepare ICC profile
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $profilename string
* @param $optlist string
*/
function PDF_load_iccprofile($pdfdoc, $profilename, $optlist);
/**
* Open image file
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $imagetype string
* @param $filename string
* @param $optlist string
*/
function PDF_load_image($pdfdoc, $imagetype, $filename, $optlist);
/**
* Make spot color
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $spotname string
*/
function PDF_makespotcolor($p, $spotname);
/**
* Set current point
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x float
* @param $y float
*/
function PDF_moveto($p, $x, $y);
/**
* Create PDFlib object
* @return resource
* @version PHP 4 >= 4.0.5, PECL
*/
function PDF_new();
/**
* Open raw CCITT image [deprecated]
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $filename string
* @param $width int
* @param $height int
* @param $BitReverse int
* @param $k int
* @param $Blackls1 int
*/
function PDF_open_ccitt($pdfdoc, $filename, $width, $height, $BitReverse, $k, $Blackls1);
/**
* Create PDF file [deprecated]
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $filename string
*/
function PDF_open_file($p, $filename);
/**
* Open GIF image [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PECL
*/
function PDF_open_gif();
/**
* Use image data [deprecated]
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $imagetype string
* @param $source string
* @param $data string
* @param $length int
* @param $width int
* @param $height int
* @param $components int
* @param $bpc int
* @param $params string
*/
function PDF_open_image($p, $imagetype, $source, $data, $length, $width, $height, $components, $bpc, $params);
/**
* Read image from file [deprecated]
* @return int
* @version PHP 3 CVS only, PHP 4, PECL
* @param $p resource
* @param $imagetype string
* @param $filename string
* @param $stringparam string
* @param $intparam int
*/
function PDF_open_image_file($p, $imagetype, $filename, $stringparam, $intparam);
/**
* Open JPEG image [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PECL
*/
function PDF_open_jpeg();
/**
* Open image created with PHP's image functions [not supported]
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PECL
* @param $p resource
* @param $image resource
*/
function PDF_open_memory_image($p, $image);
/**
* Open PDF file
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $filename string
* @param $optlist string
*/
function PDF_open_pdi($pdfdoc, $filename, $optlist);
/**
* Prepare a page
* @return int
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $doc int
* @param $pagenumber int
* @param $optlist string
*/
function PDF_open_pdi_page($p, $doc, $pagenumber, $optlist);
/**
* Open TIFF image [deprecated]
* @return &#13;
* @version PHP 4, PECL
*/
function PDF_open_tiff();
/**
* Place image on the page [deprecated]
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PECL
* @param $pdfdoc resource
* @param $image int
* @param $x float
* @param $y float
* @param $scale float
*/
function PDF_place_image($pdfdoc, $image, $x, $y, $scale);
/**
* Place PDF page [deprecated]
* @return bool
* @version PHP 4 >= 4.0.6, PECL
* @param $pdfdoc resource
* @param $page int
* @param $x float
* @param $y float
* @param $sx float
* @param $sy float
*/
function PDF_place_pdi_page($pdfdoc, $page, $x, $y, $sx, $sy);
/**
* Process imported PDF document
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $doc int
* @param $page int
* @param $optlist string
*/
function PDF_process_pdi($pdfdoc, $doc, $page, $optlist);
/**
* Draw rectangle
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x float
* @param $y float
* @param $width float
* @param $height float
*/
function PDF_rect($p, $x, $y, $width, $height);
/**
* Restore graphics state
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_restore($p);
/**
* Resume page
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_resume_page($pdfdoc, $optlist);
/**
* Rotate coordinate system
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $phi float
*/
function PDF_rotate($p, $phi);
/**
* Save graphics state
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_save($p);
/**
* Scale coordinate system
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $sx float
* @param $sy float
*/
function PDF_scale($p, $sx, $sy);
/**
* Set fill and stroke color
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $fstype string
* @param $colorspace string
* @param $c1 float
* @param $c2 float
* @param $c3 float
* @param $c4 float
*/
function PDF_setcolor($p, $fstype, $colorspace, $c1, $c2, $c3, $c4);
/**
* Set simple dash pattern
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $b float
* @param $w float
*/
function PDF_setdash($pdfdoc, $b, $w);
/**
* Set dash pattern
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_setdashpattern($pdfdoc, $optlist);
/**
* Set flatness
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $flatness float
*/
function PDF_setflat($pdfdoc, $flatness);
/**
* Set font
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $pdfdoc resource
* @param $font int
* @param $fontsize float
*/
function PDF_setfont($pdfdoc, $font, $fontsize);
/**
* Set color to gray [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $g float
*/
function PDF_setgray($p, $g);
/**
* Set fill color to gray [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $g float
*/
function PDF_setgray_fill($p, $g);
/**
* Set stroke color to gray [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $g float
*/
function PDF_setgray_stroke($p, $g);
/**
* Set linecap parameter
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $linecap int
*/
function PDF_setlinecap($p, $linecap);
/**
* Set linejoin parameter
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $value int
*/
function PDF_setlinejoin($p, $value);
/**
* Set line width
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $width float
*/
function PDF_setlinewidth($p, $width);
/**
* Set current transformation matrix
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $p resource
* @param $a float
* @param $b float
* @param $c float
* @param $d float
* @param $e float
* @param $f float
*/
function PDF_setmatrix($p, $a, $b, $c, $d, $e, $f);
/**
* Set miter limit
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $miter float
*/
function PDF_setmiterlimit($pdfdoc, $miter);
/**
* Set complicated dash pattern [deprecated]
* @return &#13;
* @version PHP 4 >= 4.0.5, PECL
*/
function PDF_setpolydash();
/**
* Set fill and stroke rgb color values [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $red float
* @param $green float
* @param $blue float
*/
function PDF_setrgbcolor($p, $red, $green, $blue);
/**
* Set fill rgb color values [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $red float
* @param $green float
* @param $blue float
*/
function PDF_setrgbcolor_fill($p, $red, $green, $blue);
/**
* Set stroke rgb color values [deprecated]
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $red float
* @param $green float
* @param $blue float
*/
function PDF_setrgbcolor_stroke($p, $red, $green, $blue);
/**
* Set border color of annotations [deprecated]
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PECL
* @param $p resource
* @param $red float
* @param $green float
* @param $blue float
*/
function PDF_set_border_color($p, $red, $green, $blue);
/**
* Set border dash style of annotations [deprecated]
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $pdfdoc resource
* @param $black float
* @param $white float
*/
function PDF_set_border_dash($pdfdoc, $black, $white);
/**
* Set border style of annotations [deprecated]
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PECL
* @param $pdfdoc resource
* @param $style string
* @param $width float
*/
function PDF_set_border_style($pdfdoc, $style, $width);
/**
* Set character spacing [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_char_spacing();
/**
* Set duration between pages [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_duration();
/**
* Activate graphics state object
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $gstate int
*/
function PDF_set_gstate($pdfdoc, $gstate);
/**
* Set horizontal text scaling [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_horiz_scaling();
/**
* Fill document info field
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $p resource
* @param $key string
* @param $value string
*/
function PDF_set_info($p, $key, $value);
/**
* Fill the author document info field [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_info_author();
/**
* Fill the creator document info field [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_info_creator();
/**
* Fill the keywords document info field [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_info_keywords();
/**
* Fill the subject document info field [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_info_subject();
/**
* Fill the title document info field [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_info_title();
/**
* Define relationships among layers
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $type string
* @param $optlist string
*/
function PDF_set_layer_dependency($pdfdoc, $type, $optlist);
/**
* Set distance between text lines [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_leading();
/**
* Set string parameter
* @return bool
* @version PHP 4, PECL
* @param $p resource
* @param $key string
* @param $value string
*/
function PDF_set_parameter($p, $key, $value);
/**
* Set text matrix [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6
*/
function PDF_set_text_matrix();
/**
* Set text position
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $x float
* @param $y float
*/
function PDF_set_text_pos($p, $x, $y);
/**
* Determine text rendering [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_text_rendering();
/**
* Set text rise [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_text_rise();
/**
* Set numerical parameter
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $p resource
* @param $key string
* @param $value float
*/
function PDF_set_value($p, $key, $value);
/**
* Set spacing between words [deprecated]
* @return &#13;
* @version PHP 3 >= 3.0.6, PHP 4, PECL
*/
function PDF_set_word_spacing();
/**
* Define blend
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $shtype string
* @param $x0 float
* @param $y0 float
* @param $x1 float
* @param $y1 float
* @param $c1 float
* @param $c2 float
* @param $c3 float
* @param $c4 float
* @param $optlist string
*/
function PDF_shading($pdfdoc, $shtype, $x0, $y0, $x1, $y1, $c1, $c2, $c3, $c4, $optlist);
/**
* Define shading pattern
* @return int
* @version PECL
* @param $pdfdoc resource
* @param $shading int
* @param $optlist string
*/
function PDF_shading_pattern($pdfdoc, $shading, $optlist);
/**
* Fill area with shading
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $shading int
*/
function PDF_shfill($pdfdoc, $shading);
/**
* Output text at current position
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $pdfdoc resource
* @param $text string
*/
function PDF_show($pdfdoc, $text);
/**
* Output text in a box [deprecated]
* @return int
* @version PHP 4, PECL
* @param $p resource
* @param $text string
* @param $left float
* @param $top float
* @param $width float
* @param $height float
* @param $mode string
* @param $feature string
*/
function PDF_show_boxed($p, $text, $left, $top, $width, $height, $mode, $feature);
/**
* Output text at given position
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $text string
* @param $x float
* @param $y float
*/
function PDF_show_xy($p, $text, $x, $y);
/**
* Skew the coordinate system
* @return bool
* @version PHP 4, PECL
* @param $p resource
* @param $alpha float
* @param $beta float
*/
function PDF_skew($p, $alpha, $beta);
/**
* Return width of text
* @return float
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $text string
* @param $font int
* @param $fontsize float
*/
function PDF_stringwidth($p, $text, $font, $fontsize);
/**
* Stroke path
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
*/
function PDF_stroke($p);
/**
* Suspend page
* @return bool
* @version PECL
* @param $pdfdoc resource
* @param $optlist string
*/
function PDF_suspend_page($pdfdoc, $optlist);
/**
* Set origin of coordinate system
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PECL
* @param $p resource
* @param $tx float
* @param $ty float
*/
function PDF_translate($p, $tx, $ty);
/**
* Convert string from UTF-16 to UTF-8
* @return string
* @version PECL
* @param $pdfdoc resource
* @param $utf16string string
*/
function PDF_utf16_to_utf8($pdfdoc, $utf16string);
/**
* Convert string from UTF-8 to UTF-16
* @return string
* @version PECL
* @param $pdfdoc resource
* @param $utf8string string
* @param $ordering string
*/
function PDF_utf8_to_utf16($pdfdoc, $utf8string, $ordering);
/**
* Shuts down the Payflow Pro library
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
*/
function pfpro_cleanup();
/**
* Initialises the Payflow Pro library
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
*/
function pfpro_init();
/**
* Process a transaction with Payflow Pro
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $parameters array
* @param $address (optional) string
* @param $port (optional) int
* @param $timeout (optional) int
* @param $proxy_address (optional) string
* @param $proxy_port (optional) int
* @param $proxy_logon (optional) string
* @param $proxy_password (optional) string
*/
function pfpro_process($parameters, $address, $port, $timeout, $proxy_address, $proxy_port, $proxy_logon, $proxy_password);
/**
* Process a raw transaction with Payflow Pro
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $parameters string
* @param $address (optional) string
* @param $port (optional) int
* @param $timeout (optional) int
* @param $proxy_address (optional) string
* @param $proxy_port (optional) int
* @param $proxy_logon (optional) string
* @param $proxy_password (optional) string
*/
function pfpro_process_raw($parameters, $address, $port, $timeout, $proxy_address, $proxy_port, $proxy_logon, $proxy_password);
/**
* Returns the version of the Payflow Pro software
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
*/
function pfpro_version();
/**
* Open persistent Internet or Unix domain socket connection
* @return resource
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $hostname string
* @param $port (optional) int
* @param &$errno (optional) int
* @param &$errstr (optional) string
* @param $timeout (optional) float
*/
function pfsockopen($hostname, $port, &$errno, &$errstr, $timeout);
/**
* Returns number of affected records (tuples)
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_affected_rows($result);
/**
* Cancel an asynchronous query
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_cancel_query($connection);
/**
* Gets the client encoding
* @return string
* @version PHP 3 CVS only, PHP 4 >= 4.0.3, PHP 5
* @param $connection resource
*/
function pg_client_encoding($connection);
/**
* Closes a PostgreSQL connection
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_close($connection);
/**
* Open a PostgreSQL connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $connection_string string
* @param $connect_type (optional) int
*/
function pg_connect($connection_string, $connect_type);
/**
* Get connection is busy or not
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_connection_busy($connection);
/**
* Reset connection (reconnect)
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_connection_reset($connection);
/**
* Get connection status
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_connection_status($connection);
/**
* Convert associative array values into suitable for SQL statement
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $assoc_array array
* @param $options (optional) int
*/
function pg_convert($connection, $table_name, $assoc_array, $options);
/**
* Insert records into a table from an array
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $rows array
* @param $delimiter (optional) string
* @param $null_as (optional) string
*/
function pg_copy_from($connection, $table_name, $rows, $delimiter, $null_as);
/**
* Copy a table to an array
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $delimiter (optional) string
* @param $null_as (optional) string
*/
function pg_copy_to($connection, $table_name, $delimiter, $null_as);
/**
* Get the database name
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_dbname($connection);
/**
* Deletes records
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $assoc_array array
* @param $options (optional) int
*/
function pg_delete($connection, $table_name, $assoc_array, $options);
/**
* Sync with PostgreSQL backend
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $connection resource
*/
function pg_end_copy($connection);
/**
* Escape a string for insertion into a bytea field
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $data string
*/
function pg_escape_bytea($data);
/**
* Escape a string for insertion into a text field
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $data string
*/
function pg_escape_string($data);
/**
* Sends a request to execute a prepared statement with given parameters, and waits for the result.
* @return resource
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $stmtname string
* @param $params array
*/
function pg_execute($connection, $stmtname, $params);
/**
* Fetches all rows from a result as an array
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $result resource
*/
function pg_fetch_all($result);
/**
* Fetches all rows in a particular result column as an array
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $result resource
* @param $column (optional) int
*/
function pg_fetch_all_columns($result, $column);
/**
* Fetch a row as an array
* @return array
* @version PHP 3 >= 3.0.1, PHP 4, PHP 5
* @param $result resource
* @param $row (optional) int
* @param $result_type (optional) int
*/
function pg_fetch_array($result, $row, $result_type);
/**
* Fetch a row as an associative array
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $result resource
* @param $row (optional) int
*/
function pg_fetch_assoc($result, $row);
/**
* Fetch a row as an object
* @return object
* @version PHP 3 >= 3.0.1, PHP 4, PHP 5
* @param $result resource
* @param $row (optional) int
* @param $result_type (optional) int
*/
function pg_fetch_object($result, $row, $result_type);
/**
* Returns values from a result resource
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $row int
* @param $field mixed
*/
function pg_fetch_result($result, $row, $field);
/**
* Get a row as an enumerated array
* @return array
* @version PHP 3 >= 3.0.1, PHP 4, PHP 5
* @param $result resource
* @param $row (optional) int
*/
function pg_fetch_row($result, $row);
/**
* Test if a field is SQL NULL
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $row int
* @param $field mixed
*/
function pg_field_is_null($result, $row, $field);
/**
* Returns the name of a field
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $field_number int
*/
function pg_field_name($result, $field_number);
/**
* Returns the field number of the named field
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $field_name string
*/
function pg_field_num($result, $field_name);
/**
* Returns the printed length
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $row_number int
* @param $field_name_or_number mixed
*/
function pg_field_prtlen($result, $row_number, $field_name_or_number);
/**
* Returns the internal storage size of the named field
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $field_number int
*/
function pg_field_size($result, $field_number);
/**
* Returns the type name for the corresponding field number
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $field_number int
*/
function pg_field_type($result, $field_number);
/**
* Returns the type ID (OID) for the corresponding field number
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $result resource
* @param $field_number int
*/
function pg_field_type_oid($result, $field_number);
/**
* Free result memory
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_free_result($result);
/**
* Gets SQL NOTIFY message
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $result_type (optional) int
*/
function pg_get_notify($connection, $result_type);
/**
* Gets the backend's process ID
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
*/
function pg_get_pid($connection);
/**
* Get asynchronous query result
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_get_result($connection);
/**
* Returns the host name associated with the connection
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_host($connection);
/**
* Insert array into table
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $assoc_array array
* @param $options (optional) int
*/
function pg_insert($connection, $table_name, $assoc_array, $options);
/**
* Get the last error message string of a connection
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_last_error($connection);
/**
* Returns the last notice message from PostgreSQL server
* @return string
* @version PHP 4 >= 4.0.6, PHP 5
* @param $connection resource
*/
function pg_last_notice($connection);
/**
* Returns the last row's OID
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_last_oid($result);
/**
* Close a large object
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
*/
function pg_lo_close($large_object);
/**
* Create a large object
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
*/
function pg_lo_create($connection);
/**
* Export a large object to file
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $oid int
* @param $pathname string
*/
function pg_lo_export($connection, $oid, $pathname);
/**
* Import a large object from file
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $pathname string
*/
function pg_lo_import($connection, $pathname);
/**
* Open a large object
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $oid int
* @param $mode string
*/
function pg_lo_open($connection, $oid, $mode);
/**
* Read a large object
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
* @param $len (optional) int
*/
function pg_lo_read($large_object, $len);
/**
* Reads an entire large object and send straight to browser
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
*/
function pg_lo_read_all($large_object);
/**
* Seeks position within a large object
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
* @param $offset int
* @param $whence (optional) int
*/
function pg_lo_seek($large_object, $offset, $whence);
/**
* Returns current seek position a of large object
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
*/
function pg_lo_tell($large_object);
/**
* Delete a large object
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $oid int
*/
function pg_lo_unlink($connection, $oid);
/**
* Write to a large object
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $large_object resource
* @param $data string
* @param $len (optional) int
*/
function pg_lo_write($large_object, $data, $len);
/**
* Get meta data for table
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
*/
function pg_meta_data($connection, $table_name);
/**
* Returns the number of fields in a result
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_num_fields($result);
/**
* Returns the number of rows in a result
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_num_rows($result);
/**
* Get the options associated with the connection
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_options($connection);
/**
* Looks up a current parameter setting of the server.
* @return string
* @version PHP 5
* @param $connection resource
* @param $param_name string
*/
function pg_parameter_status($connection, $param_name);
/**
* Open a persistent PostgreSQL connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $connection_string string
* @param $connect_type (optional) int
*/
function pg_pconnect($connection_string, $connect_type);
/**
* Ping database connection
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
*/
function pg_ping($connection);
/**
* Return the port number associated with the connection
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_port($connection);
/**
* Submits a request to create a prepared statement with the given parameters, and waits for completion.
* @return resource
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $stmtname string
* @param $query string
*/
function pg_prepare($connection, $stmtname, $query);
/**
* Send a NULL-terminated string to PostgreSQL backend
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $data string
*/
function pg_put_line($data);
/**
* Execute a query
* @return resource
* @version PHP 4 >= 4.2.0, PHP 5
* @param $query string
*/
function pg_query($query);
/**
* Submits a command to the server and waits for the result, with the ability to pass parameters separately from the SQL command text.
* @return resource
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $query string
* @param $params array
*/
function pg_query_params($connection, $query, $params);
/**
* Get error message associated with result
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
*/
function pg_result_error($result);
/**
* Returns an individual field of an error report.
* @return string
* @version PHP 5 >= 5.1.0RC1
* @param $result resource
* @param $fieldcode int
*/
function pg_result_error_field($result, $fieldcode);
/**
* Set internal row offset in result resource
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $result resource
* @param $offset int
*/
function pg_result_seek($result, $offset);
/**
* Get status of query result
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5
* @param $result resource
* @param $type (optional) int
*/
function pg_result_status($result, $type);
/**
* Select records
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $assoc_array array
* @param $options (optional) int
*/
function pg_select($connection, $table_name, $assoc_array, $options);
/**
* Sends a request to execute a prepared statement with given parameters, without waiting for the result(s).
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $stmtname string
* @param $params array
*/
function pg_send_execute($connection, $stmtname, $params);
/**
* Sends a request to create a prepared statement with the given parameters, without waiting for completion.
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $stmtname string
* @param $query string
*/
function pg_send_prepare($connection, $stmtname, $query);
/**
* Sends asynchronous query
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $connection resource
* @param $query string
*/
function pg_send_query($connection, $query);
/**
* Submits a command and separate parameters to the server without waiting for the result(s).
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $query string
* @param $params array
*/
function pg_send_query_params($connection, $query, $params);
/**
* Set the client encoding
* @return int
* @version PHP 3 CVS only, PHP 4 >= 4.0.3, PHP 5
* @param $encoding string
*/
function pg_set_client_encoding($encoding);
/**
* Determines the verbosity of messages returned by pg_last_error() and pg_result_error().
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
* @param $verbosity int
*/
function pg_set_error_verbosity($connection, $verbosity);
/**
* Enable tracing a PostgreSQL connection
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
* @param $pathname string
* @param $mode (optional) string
* @param $connection (optional) resource
*/
function pg_trace($pathname, $mode, $connection);
/**
* Returns the current in-transaction status of the server.
* @return int
* @version PHP 5 >= 5.1.0RC1
* @param $connection resource
*/
function pg_transaction_status($connection);
/**
* Return the TTY name associated with the connection
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $connection resource
*/
function pg_tty($connection);
/**
* Unescape binary for bytea type
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $data string
*/
function pg_unescape_bytea($data);
/**
* Disable tracing of a PostgreSQL connection
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
* @param $connection resource
*/
function pg_untrace($connection);
/**
* Update table
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $connection resource
* @param $table_name string
* @param $data array
* @param $condition array
* @param $options (optional) int
*/
function pg_update($connection, $table_name, $data, $condition, $options);
/**
* Returns an array with client, protocol and server version (when available)
* @return array
* @version PHP 5
* @param $connection resource
*/
function pg_version($connection);
/**
* Prints out the credits for PHP
* @return bool
* @version PHP 4, PHP 5
* @param $flag int
*/
function phpcredits($flag);
/**
* Outputs lots of PHP information
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $what int
*/
function phpinfo($what);
/**
* Gets the current PHP version
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $extension string
*/
function phpversion($extension);
/**
* Check the PHP syntax of (and execute) the specified file
* @return bool
* @version PHP 5 <= 5.0.4
* @param $file_name string
* @param &$error_message (optional) string
*/
function php_check_syntax($file_name, &$error_message);
/**
* Return a list of .ini files parsed from the additional ini dir
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
*/
function php_ini_scanned_files();
/**
* Gets the logo guid
* @return string
* @version PHP 4, PHP 5
*/
function php_logo_guid();
/**
* Returns the type of interface between web server and PHP
* @return string
* @version PHP 4 >= 4.0.1, PHP 5
*/
function php_sapi_name();
/**
* Return source with stripped comments and whitespace
* @return string
* @version PHP 5
* @param $filename string
*/
function php_strip_whitespace($filename);
/**
* Returns information about the operating system PHP is running on
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $mode string
*/
function php_uname($mode);
/**
* Get value of pi
* @return float
* @version PHP 3, PHP 4, PHP 5
*/
function pi();
/**
* Convert PNG image file to WBMP image file
* @return int
* @version PHP 4 >= 4.0.5, PHP 5
* @param $pngname string
* @param $wbmpname string
* @param $d_height int
* @param $d_width int
* @param $threshold int
*/
function png2wbmp($pngname, $wbmpname, $d_height, $d_width, $threshold);
/**
* Opens process file pointer
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $command string
* @param $mode string
*/
function popen($command, $mode);
/**
* Alias of current()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function pos();
/**
* Determine accessibility of a file
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $file string
* @param $mode (optional) int
*/
function posix_access($file, $mode);
/**
* Get path name of controlling terminal
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function posix_ctermid();
/**
* Pathname of current directory
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function posix_getcwd();
/**
* Return the effective group ID of the current process
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getegid();
/**
* Return the effective user ID of the current process
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_geteuid();
/**
* Return the real group ID of the current process
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getgid();
/**
* Return info about a group by group id
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $gid int
*/
function posix_getgrgid($gid);
/**
* Return info about a group by name
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $name string
*/
function posix_getgrnam($name);
/**
* Return the group set of the current process
* @return array
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getgroups();
/**
* Return login name
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function posix_getlogin();
/**
* Get process group id for job control
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
* @param $pid int
*/
function posix_getpgid($pid);
/**
* Return the current process group identifier
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getpgrp();
/**
* Return the current process identifier
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getpid();
/**
* Return the parent process identifier
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getppid();
/**
* Return info about a user by username
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $username string
*/
function posix_getpwnam($username);
/**
* Return info about a user by user id
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $uid int
*/
function posix_getpwuid($uid);
/**
* Return info about system resource limits
* @return array
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getrlimit();
/**
* Get the current sid of the process
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
* @param $pid int
*/
function posix_getsid($pid);
/**
* Return the real user ID of the current process
* @return int
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_getuid();
/**
* Retrieve the error number set by the last posix function that failed
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
*/
function posix_get_last_error();
/**
* Determine if a file descriptor is an interactive terminal
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $fd int
*/
function posix_isatty($fd);
/**
* Send a signal to a process
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $pid int
* @param $sig int
*/
function posix_kill($pid, $sig);
/**
* Create a fifo special file (a named pipe)
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $pathname string
* @param $mode int
*/
function posix_mkfifo($pathname, $mode);
/**
* Create a special or ordinary file (POSIX.1)
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $pathname string
* @param $mode int
* @param $major (optional) int
* @param $minor (optional) int
*/
function posix_mknod($pathname, $mode, $major, $minor);
/**
* Set the effective GID of the current process
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $gid int
*/
function posix_setegid($gid);
/**
* Set the effective UID of the current process
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $uid int
*/
function posix_seteuid($uid);
/**
* Set the GID of the current process
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $gid int
*/
function posix_setgid($gid);
/**
* Set process group id for job control
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $pid int
* @param $pgid int
*/
function posix_setpgid($pid, $pgid);
/**
* Make the current process a session leader
* @return int
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function posix_setsid();
/**
* Set the UID of the current process
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $uid int
*/
function posix_setuid($uid);
/**
* Retrieve the system error message associated with the given errno
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $errno int
*/
function posix_strerror($errno);
/**
* Get process times
* @return array
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function posix_times();
/**
* Determine terminal device name
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $fd int
*/
function posix_ttyname($fd);
/**
* Get system name
* @return array
* @version PHP 3 >= 3.0.10, PHP 4, PHP 5
*/
function posix_uname();
/**
* Exponential expression
* @return number
* @version PHP 3, PHP 4, PHP 5
* @param $base number
* @param $exp number
*/
function pow($base, $exp);
/**
* Return array entries that match the pattern
* @return array
* @version PHP 4, PHP 5
* @param $pattern string
* @param $input array
* @param $flags (optional) int
*/
function preg_grep($pattern, $input, $flags);
/**
* Perform a regular expression match
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $pattern string
* @param $subject string
* @param &$matches (optional) array
* @param $flags (optional) int
* @param $offset (optional) int
*/
function preg_match($pattern, $subject, &$matches, $flags, $offset);
/**
* Perform a global regular expression match
* @return int
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $pattern string
* @param $subject string
* @param &$matches array
* @param $flags (optional) int
* @param $offset (optional) int
*/
function preg_match_all($pattern, $subject, &$matches, $flags, $offset);
/**
* Quote regular expression characters
* @return string
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $str string
* @param $delimiter (optional) string
*/
function preg_quote($str, $delimiter);
/**
* Perform a regular expression search and replace
* @return mixed
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $pattern mixed
* @param $replacement mixed
* @param $subject mixed
* @param $limit (optional) int
* @param &$count (optional) int
*/
function preg_replace($pattern, $replacement, $subject, $limit, &$count);
/**
* Perform a regular expression search and replace using a callback
* @return mixed
* @version PHP 4 >= 4.0.5, PHP 5
* @param $pattern mixed
* @param $callback callback
* @param $subject mixed
* @param $limit (optional) int
* @param &$count (optional) int
*/
function preg_replace_callback($pattern, $callback, $subject, $limit, &$count);
/**
* Split string by a regular expression
* @return array
* @version PHP 3 >= 3.0.9, PHP 4, PHP 5
* @param $pattern string
* @param $subject string
* @param $limit (optional) int
* @param $flags (optional) int
*/
function preg_split($pattern, $subject, $limit, $flags);
/**
* Rewind the internal array pointer
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function prev(&$array);
/**
* Output a string
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $arg string
*/
function print($arg);
/**
* Deletes the printer's spool file
* @return 
* @version PECL
* @param $handle resource
*/
function printer_abort($handle);
/**
* Close an open printer connection
* @return 
* @version PECL
* @param $handle resource
*/
function printer_close($handle);
/**
* Create a new brush
* @return resource
* @version PECL
* @param $style int
* @param $color string
*/
function printer_create_brush($style, $color);
/**
* Create a new device context
* @return 
* @version PECL
* @param $handle resource
*/
function printer_create_dc($handle);
/**
* Create a new font
* @return resource
* @version PECL
* @param $face string
* @param $height int
* @param $width int
* @param $font_weight int
* @param $italic bool
* @param $underline bool
* @param $strikeout bool
* @param $orientation int
*/
function printer_create_font($face, $height, $width, $font_weight, $italic, $underline, $strikeout, $orientation);
/**
* Create a new pen
* @return resource
* @version PECL
* @param $style int
* @param $width int
* @param $color string
*/
function printer_create_pen($style, $width, $color);
/**
* Delete a brush
* @return 
* @version PECL
* @param $handle resource
*/
function printer_delete_brush($handle);
/**
* Delete a device context
* @return bool
* @version PECL
* @param $handle resource
*/
function printer_delete_dc($handle);
/**
* Delete a font
* @return 
* @version PECL
* @param $handle resource
*/
function printer_delete_font($handle);
/**
* Delete a pen
* @return 
* @version PECL
* @param $handle resource
*/
function printer_delete_pen($handle);
/**
* Draw a bmp
* @return bool
* @version PECL
* @param $handle resource
* @param $filename string
* @param $x int
* @param $y int
* @param $width (optional) int
* @param $height (optional) int
*/
function printer_draw_bmp($handle, $filename, $x, $y, $width, $height);
/**
* Draw a chord
* @return 
* @version PECL
* @param $handle resource
* @param $rec_x int
* @param $rec_y int
* @param $rec_x1 int
* @param $rec_y1 int
* @param $rad_x int
* @param $rad_y int
* @param $rad_x1 int
* @param $rad_y1 int
*/
function printer_draw_chord($handle, $rec_x, $rec_y, $rec_x1, $rec_y1, $rad_x, $rad_y, $rad_x1, $rad_y1);
/**
* Draw an ellipse
* @return 
* @version PECL
* @param $handle resource
* @param $ul_x int
* @param $ul_y int
* @param $lr_x int
* @param $lr_y int
*/
function printer_draw_elipse($handle, $ul_x, $ul_y, $lr_x, $lr_y);
/**
* Draw a line
* @return 
* @version PECL
* @param $printer_handle resource
* @param $from_x int
* @param $from_y int
* @param $to_x int
* @param $to_y int
*/
function printer_draw_line($printer_handle, $from_x, $from_y, $to_x, $to_y);
/**
* Draw a pie
* @return 
* @version PECL
* @param $handle resource
* @param $rec_x int
* @param $rec_y int
* @param $rec_x1 int
* @param $rec_y1 int
* @param $rad1_x int
* @param $rad1_y int
* @param $rad2_x int
* @param $rad2_y int
*/
function printer_draw_pie($handle, $rec_x, $rec_y, $rec_x1, $rec_y1, $rad1_x, $rad1_y, $rad2_x, $rad2_y);
/**
* Draw a rectangle
* @return 
* @version PECL
* @param $handle resource
* @param $ul_x int
* @param $ul_y int
* @param $lr_x int
* @param $lr_y int
*/
function printer_draw_rectangle($handle, $ul_x, $ul_y, $lr_x, $lr_y);
/**
* Draw a rectangle with rounded corners
* @return 
* @version PECL
* @param $handle resource
* @param $ul_x int
* @param $ul_y int
* @param $lr_x int
* @param $lr_y int
* @param $width int
* @param $height int
*/
function printer_draw_roundrect($handle, $ul_x, $ul_y, $lr_x, $lr_y, $width, $height);
/**
* Draw text
* @return 
* @version PECL
* @param $printer_handle resource
* @param $text string
* @param $x int
* @param $y int
*/
function printer_draw_text($printer_handle, $text, $x, $y);
/**
* Close document
* @return bool
* @version PECL
* @param $handle resource
*/
function printer_end_doc($handle);
/**
* Close active page
* @return bool
* @version PECL
* @param $handle resource
*/
function printer_end_page($handle);
/**
* Retrieve printer configuration data
* @return mixed
* @version PECL
* @param $handle resource
* @param $option string
*/
function printer_get_option($handle, $option);
/**
* Return an array of printers attached to the server
* @return array
* @version PECL
* @param $enumtype int
* @param $name (optional) string
* @param $level (optional) int
*/
function printer_list($enumtype, $name, $level);
/**
* Get logical font height
* @return int
* @version PECL
* @param $handle resource
* @param $height int
*/
function printer_logical_fontheight($handle, $height);
/**
* Open connection to a printer
* @return resource
* @version PECL
* @param $devicename string
*/
function printer_open($devicename);
/**
* Select a brush
* @return 
* @version PECL
* @param $printer_handle resource
* @param $brush_handle resource
*/
function printer_select_brush($printer_handle, $brush_handle);
/**
* Select a font
* @return 
* @version PECL
* @param $printer_handle resource
* @param $font_handle resource
*/
function printer_select_font($printer_handle, $font_handle);
/**
* Select a pen
* @return 
* @version PECL
* @param $printer_handle resource
* @param $pen_handle resource
*/
function printer_select_pen($printer_handle, $pen_handle);
/**
* Configure the printer connection
* @return bool
* @version PECL
* @param $handle resource
* @param $option int
* @param $value mixed
*/
function printer_set_option($handle, $option, $value);
/**
* Start a new document
* @return bool
* @version PECL
* @param $handle resource
* @param $document (optional) string
*/
function printer_start_doc($handle, $document);
/**
* Start a new page
* @return bool
* @version PECL
* @param $handle resource
*/
function printer_start_page($handle);
/**
* Write data to the printer
* @return bool
* @version PECL
* @param $handle resource
* @param $content string
*/
function printer_write($handle, $content);
/**
* Output a formatted string
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function printf($format, $args, $params1);
/**
* Prints human-readable information about a variable
* @return bool
* @version PHP 4, PHP 5
* @param $expression mixed
* @param $return (optional) bool
*/
function print_r($expression, $return);
/**
* Close a process opened by proc_open() and return the exit code of that process.
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $process resource
*/
function proc_close($process);
/**
* Get information about a process opened by proc_open()
* @return array
* @version PHP 5
* @param $process resource
*/
function proc_get_status($process);
/**
* Change the priority of the current process
* @return bool
* @version PHP 5
* @param $increment int
*/
function proc_nice($increment);
/**
* Execute a command and open file pointers for input/output
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $cmd string
* @param $descriptorspec array
* @param &$pipes array
* @param $cwd (optional) string
* @param $env (optional) array
* @param $other_options (optional) array
*/
function proc_open($cmd, $descriptorspec, &$pipes, $cwd, $env, $other_options);
/**
* Kills a process opened by proc_open
* @return int
* @version PHP 5
* @param $process resource
* @param $signal (optional) int
*/
function proc_terminate($process, $signal);
/**
* Checks if the object or class has a property
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $class mixed
* @param $property string
*/
function property_exists($class, $property);
/**
* Add the word to a personal wordlist
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $word string
*/
function pspell_add_to_personal($dictionary_link, $word);
/**
* Add the word to the wordlist in the current session
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $word string
*/
function pspell_add_to_session($dictionary_link, $word);
/**
* Check a word
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $word string
*/
function pspell_check($dictionary_link, $word);
/**
* Clear the current session
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
*/
function pspell_clear_session($dictionary_link);
/**
* Create a config used to open a dictionary
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $language string
* @param $spelling (optional) string
* @param $jargon (optional) string
* @param $encoding (optional) string
*/
function pspell_config_create($language, $spelling, $jargon, $encoding);
/**
* location of language data files
* @return bool
* @version PHP 5
* @param $conf int
* @param $directory string
*/
function pspell_config_data_dir($conf, $directory);
/**
* Location of the main word list
* @return bool
* @version PHP 5
* @param $conf int
* @param $directory string
*/
function pspell_config_dict_dir($conf, $directory);
/**
* Ignore words less than N characters long
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $n int
*/
function pspell_config_ignore($dictionary_link, $n);
/**
* Change the mode number of suggestions returned
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $mode int
*/
function pspell_config_mode($dictionary_link, $mode);
/**
* Set a file that contains personal wordlist
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $file string
*/
function pspell_config_personal($dictionary_link, $file);
/**
* Set a file that contains replacement pairs
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $file string
*/
function pspell_config_repl($dictionary_link, $file);
/**
* Consider run-together words as valid compounds
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $flag bool
*/
function pspell_config_runtogether($dictionary_link, $flag);
/**
* Determine whether to save a replacement pairs list along with the wordlist
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $flag bool
*/
function pspell_config_save_repl($dictionary_link, $flag);
/**
* Load a new dictionary
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $language string
* @param $spelling (optional) string
* @param $jargon (optional) string
* @param $encoding (optional) string
* @param $mode (optional) int
*/
function pspell_new($language, $spelling, $jargon, $encoding, $mode);
/**
* Load a new dictionary with settings based on a given config
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $config int
*/
function pspell_new_config($config);
/**
* Load a new dictionary with personal wordlist
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $personal string
* @param $language string
* @param $spelling (optional) string
* @param $jargon (optional) string
* @param $encoding (optional) string
* @param $mode (optional) int
*/
function pspell_new_personal($personal, $language, $spelling, $jargon, $encoding, $mode);
/**
* Save the personal wordlist to a file
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
*/
function pspell_save_wordlist($dictionary_link);
/**
* Store a replacement pair for a word
* @return bool
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $misspelled string
* @param $correct string
*/
function pspell_store_replacement($dictionary_link, $misspelled, $correct);
/**
* Suggest spellings of a word
* @return array
* @version PHP 4 >= 4.0.2, PHP 5
* @param $dictionary_link int
* @param $word string
*/
function pspell_suggest($dictionary_link, $word);
/**
* Add bookmark to current page
* @return int
* @version PECL
* @param $psdoc resource
* @param $text string
* @param $parent (optional) int
* @param $open (optional) int
*/
function ps_add_bookmark($psdoc, $text, $parent, $open);
/**
* Adds link which launches file
* @return bool
* @version PECL
* @param $psdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $filename string
*/
function ps_add_launchlink($psdoc, $llx, $lly, $urx, $ury, $filename);
/**
* Adds link to a page in the same document
* @return bool
* @version PECL
* @param $psdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $page int
* @param $dest string
*/
function ps_add_locallink($psdoc, $llx, $lly, $urx, $ury, $page, $dest);
/**
* Adds note to current page
* @return bool
* @version PECL
* @param $psdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $contents string
* @param $title string
* @param $icon string
* @param $open int
*/
function ps_add_note($psdoc, $llx, $lly, $urx, $ury, $contents, $title, $icon, $open);
/**
* Adds link to a page in a second pdf document
* @return bool
* @version PECL
* @param $psdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $filename string
* @param $page int
* @param $dest string
*/
function ps_add_pdflink($psdoc, $llx, $lly, $urx, $ury, $filename, $page, $dest);
/**
* Adds link to a web location
* @return bool
* @version PECL
* @param $psdoc resource
* @param $llx float
* @param $lly float
* @param $urx float
* @param $ury float
* @param $url string
*/
function ps_add_weblink($psdoc, $llx, $lly, $urx, $ury, $url);
/**
* Draws an arc counterclockwise
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
* @param $radius float
* @param $alpha float
* @param $beta float
*/
function ps_arc($psdoc, $x, $y, $radius, $alpha, $beta);
/**
* Start a new page
* @return bool
* @version PECL
* @param $psdoc resource
* @param $width float
* @param $height float
*/
function ps_begin_page($psdoc, $width, $height);
/**
* Start a new pattern
* @return int
* @version PECL
* @param $psdoc resource
* @param $width float
* @param $height float
* @param $xstep float
* @param $ystep float
* @param $painttype int
*/
function ps_begin_pattern($psdoc, $width, $height, $xstep, $ystep, $painttype);
/**
* Start a new template
* @return int
* @version PECL
* @param $psdoc resource
* @param $width float
* @param $height float
*/
function ps_begin_template($psdoc, $width, $height);
/**
* Draws a circle
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
* @param $radius float
*/
function ps_circle($psdoc, $x, $y, $radius);
/**
* Clips drawing to current path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_clip($psdoc);
/**
* Closes a PostScript document
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_close($psdoc);
/**
* Closes path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_closepath($psdoc);
/**
* Closes and strokes path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_closepath_stroke($psdoc);
/**
* Closes image and frees memory
* @return 
* @version PECL
* @param $psdoc resource
* @param $imageid int
*/
function ps_close_image($psdoc, $imageid);
/**
* Continue text in next line
* @return bool
* @version PECL
* @param $psdoc resource
* @param $text string
*/
function ps_continue_text($psdoc, $text);
/**
* Draws a curve
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $x3 float
* @param $y3 float
*/
function ps_curveto($psdoc, $x1, $y1, $x2, $y2, $x3, $y3);
/**
* Deletes all resources of a PostScript document
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_delete($psdoc);
/**
* End a page
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_end_page($psdoc);
/**
* End a pattern
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_end_pattern($psdoc);
/**
* End a template
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_end_template($psdoc);
/**
* Fills the current path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_fill($psdoc);
/**
* Fills and strokes the current path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_fill_stroke($psdoc);
/**
* Loads a font
* @return int
* @version PECL
* @param $psdoc resource
* @param $fontname string
* @param $encoding string
* @param $embed (optional) bool
*/
function ps_findfont($psdoc, $fontname, $encoding, $embed);
/**
* Fetches the full buffer containig the generated PS data
* @return string
* @version PECL
* @param $psdoc resource
*/
function ps_get_buffer($psdoc);
/**
* Gets certain parameters
* @return string
* @version PECL
* @param $psdoc resource
* @param $name string
* @param $modifier (optional) float
*/
function ps_get_parameter($psdoc, $name, $modifier);
/**
* Gets certain values
* @return float
* @version PECL
* @param $psdoc resource
* @param $name string
* @param $modifier (optional) float
*/
function ps_get_value($psdoc, $name, $modifier);
/**
* Hyphenates a word
* @return array
* @version PECL
* @param $psdoc resource
* @param $text string
*/
function ps_hyphenate($psdoc, $text);
/**
* Draws a line
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
*/
function ps_lineto($psdoc, $x, $y);
/**
* Create spot color
* @return int
* @version PECL
* @param $psdoc resource
* @param $name string
* @param $reserved (optional) float
*/
function ps_makespotcolor($psdoc, $name, $reserved);
/**
* Sets current point
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
*/
function ps_moveto($psdoc, $x, $y);
/**
* Creates a new PostScript document object
* @return resource
* @version PECL
*/
function ps_new();
/**
* Opens a file for output
* @return bool
* @version PECL
* @param $psdoc resource
* @param $filename (optional) string
*/
function ps_open_file($psdoc, $filename);
/**
* Reads an image for later placement
* @return int
* @version PECL
* @param $psdoc resource
* @param $type string
* @param $source string
* @param $data string
* @param $lenght int
* @param $width int
* @param $height int
* @param $components int
* @param $bpc int
* @param $params string
*/
function ps_open_image($psdoc, $type, $source, $data, $lenght, $width, $height, $components, $bpc, $params);
/**
* Opens image from file
* @return int
* @version PECL
* @param $psdoc resource
* @param $type string
* @param $filename string
* @param $stringparam (optional) string
* @param $intparam (optional) int
*/
function ps_open_image_file($psdoc, $type, $filename, $stringparam, $intparam);
/**
* Places image on the page
* @return bool
* @version PECL
* @param $psdoc resource
* @param $imageid int
* @param $x float
* @param $y float
* @param $scale float
*/
function ps_place_image($psdoc, $imageid, $x, $y, $scale);
/**
* Draws a rectangle
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
* @param $width float
* @param $height float
*/
function ps_rect($psdoc, $x, $y, $width, $height);
/**
* Restore previously save context
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_restore($psdoc);
/**
* Sets rotation factor
* @return bool
* @version PECL
* @param $psdoc resource
* @param $rot float
*/
function ps_rotate($psdoc, $rot);
/**
* Save current context
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_save($psdoc);
/**
* Sets scaling factor
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
*/
function ps_scale($psdoc, $x, $y);
/**
* Sets current color
* @return bool
* @version PECL
* @param $psdoc resource
* @param $type string
* @param $colorspace string
* @param $c1 float
* @param $c2 float
* @param $c3 float
* @param $c4 float
*/
function ps_setcolor($psdoc, $type, $colorspace, $c1, $c2, $c3, $c4);
/**
* Sets appearance of a dashed line
* @return bool
* @version PECL
* @param $psdoc resource
* @param $on float
* @param $off float
*/
function ps_setdash($psdoc, $on, $off);
/**
* Sets flatness
* @return bool
* @version PECL
* @param $psdoc resource
* @param $value float
*/
function ps_setflat($psdoc, $value);
/**
* Sets font to use for following output
* @return bool
* @version PECL
* @param $psdoc resource
* @param $fontid int
* @param $size float
*/
function ps_setfont($psdoc, $fontid, $size);
/**
* Sets appearance of line ends
* @return bool
* @version PECL
* @param $psdoc resource
* @param $type int
*/
function ps_setlinecap($psdoc, $type);
/**
* Sets how contected lines are joined
* @return bool
* @version PECL
* @param $psdoc resource
* @param $type int
*/
function ps_setlinejoin($psdoc, $type);
/**
* Sets width of a line
* @return bool
* @version PECL
* @param $psdoc resource
* @param $width float
*/
function ps_setlinewidth($psdoc, $width);
/**
* Sets the miter limit
* @return bool
* @version PECL
* @param $psdoc resource
* @param $value float
*/
function ps_setmiterlimit($psdoc, $value);
/**
* Sets appearance of a dashed line
* @return bool
* @version PECL
* @param $psdoc resource
* @param $arr float
*/
function ps_setpolydash($psdoc, $arr);
/**
* Sets color of border for annotations
* @return bool
* @version PECL
* @param $psdoc resource
* @param $red float
* @param $green float
* @param $blue float
*/
function ps_set_border_color($psdoc, $red, $green, $blue);
/**
* Sets length of dashes for border of annotations
* @return bool
* @version PECL
* @param $psdoc resource
* @param $black float
* @param $white float
*/
function ps_set_border_dash($psdoc, $black, $white);
/**
* Sets border style of annotations
* @return bool
* @version PECL
* @param $psdoc resource
* @param $style string
* @param $width float
*/
function ps_set_border_style($psdoc, $style, $width);
/**
* Sets information fields of document
* @return bool
* @version PECL
* @param $p resource
* @param $key string
* @param $val string
*/
function ps_set_info($p, $key, $val);
/**
* Sets certain parameters
* @return bool
* @version PECL
* @param $psdoc resource
* @param $name string
* @param $value string
*/
function ps_set_parameter($psdoc, $name, $value);
/**
* Sets position for text output
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
*/
function ps_set_text_pos($psdoc, $x, $y);
/**
* Sets certain values
* @return bool
* @version PECL
* @param $psdoc resource
* @param $name string
* @param $value float
*/
function ps_set_value($psdoc, $name, $value);
/**
* Creates a shading for later use
* @return int
* @version PECL
* @param $psdoc resource
* @param $type string
* @param $x0 float
* @param $y0 float
* @param $x1 float
* @param $y1 float
* @param $c1 float
* @param $c2 float
* @param $c3 float
* @param $c4 float
* @param $optlist string
*/
function ps_shading($psdoc, $type, $x0, $y0, $x1, $y1, $c1, $c2, $c3, $c4, $optlist);
/**
* Creates a pattern based on a shading
* @return int
* @version PECL
* @param $psdoc resource
* @param $shadingid int
* @param $optlist string
*/
function ps_shading_pattern($psdoc, $shadingid, $optlist);
/**
* Fills an area with a shading
* @return bool
* @version PECL
* @param $psdoc resource
* @param $shadingid int
*/
function ps_shfill($psdoc, $shadingid);
/**
* Output text
* @return bool
* @version PECL
* @param $psdoc resource
* @param $text string
*/
function ps_show($psdoc, $text);
/**
* Output text in a box
* @return int
* @version PECL
* @param $psdoc resource
* @param $text string
* @param $left float
* @param $bottom float
* @param $width float
* @param $height float
* @param $hmode string
* @param $feature (optional) string
*/
function ps_show_boxed($psdoc, $text, $left, $bottom, $width, $height, $hmode, $feature);
/**
* Output text at given position
* @return bool
* @version PECL
* @param $psdoc resource
* @param $text string
* @param $x float
* @param $y float
*/
function ps_show_xy($psdoc, $text, $x, $y);
/**
* Gets width of a string
* @return float
* @version PECL
* @param $psdoc resource
* @param $text string
* @param $fontid (optional) int
* @param $size (optional) float
*/
function ps_stringwidth($psdoc, $text, $fontid, $size);
/**
* Gets geometry of a string
* @return array
* @version PECL
* @param $psdoc resource
* @param $text string
* @param $fontid (optional) int
* @param $size (optional) float
*/
function ps_string_geometry($psdoc, $text, $fontid, $size);
/**
* Draws the current path
* @return bool
* @version PECL
* @param $psdoc resource
*/
function ps_stroke($psdoc);
/**
* Output a glyph
* @return bool
* @version PECL
* @param $psdoc resource
* @param $ord int
*/
function ps_symbol($psdoc, $ord);
/**
* Gets name of a glyph
* @return string
* @version PECL
* @param $psdoc resource
* @param $ord int
* @param $fontid (optional) int
*/
function ps_symbol_name($psdoc, $ord, $fontid);
/**
* Gets width of a glyph
* @return float
* @version PECL
* @param $psdoc resource
* @param $ord int
* @param $fontid (optional) int
* @param $size (optional) float
*/
function ps_symbol_width($psdoc, $ord, $fontid, $size);
/**
* Sets translation
* @return bool
* @version PECL
* @param $psdoc resource
* @param $x float
* @param $y float
*/
function ps_translate($psdoc, $x, $y);
/**
* Sets the value of an environment variable
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $setting string
*/
function putenv($setting);
/**
* Closes a paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
*/
function px_close($pxdoc);
/**
* Create a new paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $file resource
* @param $fielddesc array
*/
function px_create_fp($pxdoc, $file, $fielddesc);
/**
* Deletes resource of paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
*/
function px_delete($pxdoc);
/**
* Deletes record from paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $num int
*/
function px_delete_record($pxdoc, $num);
/**
* Returns the specification of a single field
* @return array
* @version PECL
* @param $pxdoc resource
* @param $fieldno int
*/
function px_get_field($pxdoc, $fieldno);
/**
* Return lots of information about a paradox file
* @return array
* @version PECL
* @param $pxdoc resource
*/
function px_get_info($pxdoc);
/**
* Gets a parameter
* @return string
* @version PECL
* @param $pxdoc resource
* @param $name string
*/
function px_get_parameter($pxdoc, $name);
/**
* Returns record of paradox database
* @return array
* @version PECL
* @param $pxdoc resource
* @param $num int
* @param $mode (optional) int
*/
function px_get_record($pxdoc, $num, $mode);
/**
* Returns the database schema
* @return array
* @version PECL
* @param $pxdoc resource
* @param $mode (optional) int
*/
function px_get_schema($pxdoc, $mode);
/**
* Gets a value
* @return float
* @version PECL
* @param $pxdoc resource
* @param $name string
*/
function px_get_value($pxdoc, $name);
/**
* Inserts record into paradox database
* @return int
* @version PECL
* @param $pxdoc resource
* @param $data array
*/
function px_insert_record($pxdoc, $data);
/**
* Create a new paradox object
* @return resource
* @version PECL
*/
function px_new();
/**
* Returns number of fields in a database
* @return int
* @version PECL
* @param $pxdoc resource
*/
function px_numfields($pxdoc);
/**
* Returns number of records in a database
* @return int
* @version PECL
* @param $pxdoc resource
*/
function px_numrecords($pxdoc);
/**
* Open paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $file resource
*/
function px_open_fp($pxdoc, $file);
/**
* Stores record into paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $record array
* @param $recpos (optional) int
*/
function px_put_record($pxdoc, $record, $recpos);
/**
* Returns record of paradox database
* @return array
* @version PECL
* @param $pxdoc resource
* @param $num int
* @param $mode (optional) int
*/
function px_retrieve_record($pxdoc, $num, $mode);
/**
* Sets the file where blobs are read from
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $filename string
*/
function px_set_blob_file($pxdoc, $filename);
/**
* Sets a parameter
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $name string
* @param $value string
*/
function px_set_parameter($pxdoc, $name, $value);
/**
* Sets the name of a table (deprecated)
* @return 
* @version PECL
* @param $pxdoc resource
* @param $name string
*/
function px_set_tablename($pxdoc, $name);
/**
* Sets the encoding for character fields (deprecated)
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $encoding string
*/
function px_set_targetencoding($pxdoc, $encoding);
/**
* Sets a value
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $name string
* @param $value float
*/
function px_set_value($pxdoc, $name, $value);
/**
* Converts the timestamp into a string.
* @return string
* @version PECL
* @param $pxdoc resource
* @param $value float
* @param $format string
*/
function px_timestamp2string($pxdoc, $value, $format);
/**
* Updates record in paradox database
* @return bool
* @version PECL
* @param $pxdoc resource
* @param $data array
* @param $num int
*/
function px_update_record($pxdoc, $data, $num);
/**
* Returns the error string from the last QDOM operation or FALSE if no errors occurred
* @return string
* @version PHP 4 >= 4.0.5, PECL
*/
function qdom_error();
/**
* Creates a tree of an XML string
* @return QDomDocument
* @version PHP 4 >= 4.0.4, PECL
* @param $doc string
*/
function qdom_tree($doc);
/**
* Convert a quoted-printable string to an 8 bit string
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $str string
*/
function quoted_printable_decode($str);
/**
* Quote meta characters
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function quotemeta($str);
/**
* Converts the radian number to the equivalent number in degrees
* @return float
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $number float
*/
function rad2deg($number);
/**
* Creates a Radius handle for accounting
* @return resource
* @version PECL
*/
function radius_acct_open();
/**
* Adds a server
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $hostname string
* @param $port int
* @param $secret string
* @param $timeout int
* @param $max_tries int
*/
function radius_add_server($radius_handle, $hostname, $port, $secret, $timeout, $max_tries);
/**
* Creates a Radius handle for authentication
* @return resource
* @version PECL
*/
function radius_auth_open();
/**
* Frees all ressources
* @return bool
* @version PECL
* @param $radius_handle resource
*/
function radius_close($radius_handle);
/**
* Causes the library to read the given configuration file
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $file string
*/
function radius_config($radius_handle, $file);
/**
* Create accounting or authentication request
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $type int
*/
function radius_create_request($radius_handle, $type);
/**
* Converts raw data to IP-Address
* @return string
* @version PECL
* @param $data string
*/
function radius_cvt_addr($data);
/**
* Converts raw data to integer
* @return int
* @version PECL
* @param $data string
*/
function radius_cvt_int($data);
/**
* Converts raw data to string
* @return string
* @version PECL
* @param $data string
*/
function radius_cvt_string($data);
/**
* Demangles data
* @return string
* @version PECL
* @param $radius_handle resource
* @param $mangled string
*/
function radius_demangle($radius_handle, $mangled);
/**
* Derives mppe-keys from mangled data
* @return string
* @version PECL
* @param $radius_handle resource
* @param $mangled string
*/
function radius_demangle_mppe_key($radius_handle, $mangled);
/**
* Extracts an attribute
* @return mixed
* @version PECL
* @param $radius_handle resource
*/
function radius_get_attr($radius_handle);
/**
* Extracts a vendor specific attribute
* @return array
* @version PECL
* @param $data string
*/
function radius_get_vendor_attr($data);
/**
* Attaches an IP-Address attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $type int
* @param $addr string
*/
function radius_put_addr($radius_handle, $type, $addr);
/**
* Attaches a binary attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $type int
* @param $value string
*/
function radius_put_attr($radius_handle, $type, $value);
/**
* Attaches an integer attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $type int
* @param $value int
*/
function radius_put_int($radius_handle, $type, $value);
/**
* Attaches a string attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $type int
* @param $value string
*/
function radius_put_string($radius_handle, $type, $value);
/**
* Attaches a vendor specific IP-Address attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $vendor int
* @param $type int
* @param $addr string
*/
function radius_put_vendor_addr($radius_handle, $vendor, $type, $addr);
/**
* Attaches a vendor specific binary attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $vendor int
* @param $type int
* @param $value string
*/
function radius_put_vendor_attr($radius_handle, $vendor, $type, $value);
/**
* Attaches a vendor specific integer attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $vendor int
* @param $type int
* @param $value int
*/
function radius_put_vendor_int($radius_handle, $vendor, $type, $value);
/**
* Attaches a vendor specific string attribute
* @return bool
* @version PECL
* @param $radius_handle resource
* @param $vendor int
* @param $type int
* @param $value string
*/
function radius_put_vendor_string($radius_handle, $vendor, $type, $value);
/**
* Returns the request authenticator
* @return string
* @version PECL
* @param $radius_handle resource
*/
function radius_request_authenticator($radius_handle);
/**
* Sends the request and waites for a reply
* @return int
* @version PECL
* @param $radius_handle resource
*/
function radius_send_request($radius_handle);
/**
* Returns the shared secret
* @return string
* @version PECL
* @param $radius_handle resource
*/
function radius_server_secret($radius_handle);
/**
* Returns an error message
* @return string
* @version PECL
* @param $radius_handle resource
*/
function radius_strerror($radius_handle);
/**
* Generate a random integer
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $min int
* @param $max (optional) int
*/
function rand($min, $max);
/**
* Create an array containing a range of elements
* @return array
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $low mixed
* @param $high mixed
* @param $step (optional) number
*/
function range($low, $high, $step);
/**
* Decode URL-encoded strings
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function rawurldecode($str);
/**
* URL-encode according to RFC 1738
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function rawurlencode($str);
/**
* Read entry from directory handle
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $dir_handle resource
*/
function readdir($dir_handle);
/**
* Outputs a file
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $use_include_path (optional) bool
* @param $context (optional) resource
*/
function readfile($filename, $use_include_path, $context);
/**
* Output a gz-file
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $use_include_path (optional) int
*/
function readgzfile($filename, $use_include_path);
/**
* Reads a line
* @return string
* @version PHP 4, PHP 5
* @param $prompt string
*/
function readline($prompt);
/**
* Adds a line to the history
* @return bool
* @version PHP 4, PHP 5
* @param $line string
*/
function readline_add_history($line);
/**
* Initializes the readline callback interface and terminal, prints the prompt and returns immediately
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $prompt string
* @param $callback callback
*/
function readline_callback_handler_install($prompt, $callback);
/**
* Removes a previously installed callback handler and restores terminal settings
* @return bool
* @version PHP 5 >= 5.1.0RC1
*/
function readline_callback_handler_remove();
/**
* Reads a character and informs the readline callback interface when a line is received
* @return 
* @version PHP 5 >= 5.1.0RC1
*/
function readline_callback_read_char();
/**
* Clears the history
* @return bool
* @version PHP 4, PHP 5
*/
function readline_clear_history();
/**
* Registers a completion function
* @return bool
* @version PHP 4, PHP 5
* @param $function callback
*/
function readline_completion_function($function);
/**
* Gets/sets various internal readline variables
* @return mixed
* @version PHP 4, PHP 5
* @param $varname string
* @param $newvalue (optional) string
*/
function readline_info($varname, $newvalue);
/**
* Lists the history
* @return array
* @version PHP 4, PHP 5
*/
function readline_list_history();
/**
* Inform readline that the cursor has moved to a new line
* @return 
* @version PHP 5 >= 5.1.0RC1
*/
function readline_on_new_line();
/**
* Reads the history
* @return bool
* @version PHP 4, PHP 5
* @param $filename string
*/
function readline_read_history($filename);
/**
* Ask readline to redraw the display
* @return 
* @version PHP 5 >= 5.1.0RC1
*/
function readline_redisplay();
/**
* Writes the history
* @return bool
* @version PHP 4, PHP 5
* @param $filename string
*/
function readline_write_history($filename);
/**
* Returns the target of a symbolic link
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $path string
*/
function readlink($path);
/**
* Alias of exif_read_data()
* @return &#13;
* @version PHP 4 >= 4.0.1, PHP 5
*/
function read_exif_data();
/**
* Returns canonicalized absolute pathname
* @return string
* @version PHP 4, PHP 5
* @param $path string
*/
function realpath($path);
/**
* Alias of recode_string()
* @return &#13;
* @version PHP 4, PHP 5
*/
function recode();
/**
* Recode from file to file according to recode request
* @return bool
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $request string
* @param $input resource
* @param $output resource
*/
function recode_file($request, $input, $output);
/**
* Recode a string according to a recode request
* @return string
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
* @param $request string
* @param $string string
*/
function recode_string($request, $string);
/**
* Register a function for execution on shutdown
* @return 
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param $function callback
* @param $parameter (optional) mixed
* @param $params1 (optional) mixed
*/
function register_shutdown_function($function, $parameter, $params1);
/**
* Register a function for execution on each tick
* @return bool
* @version PHP 4 >= 4.0.3, PHP 5
* @param $function callback
* @param $arg (optional) mixed
* @param $params1 (optional) mixed
*/
function register_tick_function($function, $arg, $params1);
/**
* Renames a file or directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $oldname string
* @param $newname string
* @param $context (optional) resource
*/
function rename($oldname, $newname, $context);
/**
* Renames orig_name to new_name in the global function_table
* @return bool
* @version PECL
* @param $original_name string
* @param $new_name string
*/
function rename_function($original_name, $new_name);
/**
* N/A
* @return N/A
* @version N/A
*/
function require()();
/**
* N/A
* @return N/A
* @version N/A
*/
function require_once()();
/**
* Set the internal pointer of an array to its first element
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
*/
function reset(&$array);
/**
* Restores the previous error handler function
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
*/
function restore_error_handler();
/**
* Restores the previously defined exception handler function
* @return bool
* @version PHP 5
*/
function restore_exception_handler();
/**
* Restores the value of the include_path configuration option
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
*/
function restore_include_path();
/**
* N/A
* @return N/A
* @version &#13; If called from within a function, the return()
*/
function return();
/**
* Rewind the position of a file pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $handle resource
*/
function rewind($handle);
/**
* Rewind directory handle
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $dir_handle resource
*/
function rewinddir($dir_handle);
/**
* Removes directory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $dirname string
* @param $context (optional) resource
*/
function rmdir($dirname, $context);
/**
* Rounds a float
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $val float
* @param $precision (optional) int
*/
function round($val, $precision);
/**
* Sort an array in reverse order
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function rsort(&$array, $sort_flags);
/**
* Strip whitespace (or other characters) from the end of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $charlist (optional) string
*/
function rtrim($str, $charlist);
/**
* Convert a base class to an inherited class, add ancestral methods when appropriate
* @return bool
* @version PECL
* @param $classname string
* @param $parentname string
*/
function runkit_class_adopt($classname, $parentname);
/**
* Convert an inherited class to a base class, removes any method whose scope is ancestral
* @return bool
* @version PECL
* @param $classname string
*/
function runkit_class_emancipate($classname);
/**
* Similar to define(), but allows defining in class definitions as well
* @return bool
* @version PECL
* @param $constname string
* @param $value mixed
*/
function runkit_constant_add($constname, $value);
/**
* Redefine an already defined constant
* @return bool
* @version PECL
* @param $constname string
* @param $newvalue mixed
*/
function runkit_constant_redefine($constname, $newvalue);
/**
* Remove/Delete an already defined constant
* @return bool
* @version PECL
* @param $constname string
*/
function runkit_constant_remove($constname);
/**
* Add a new function, similar to create_function()
* @return bool
* @version PECL
* @param $funcname string
* @param $arglist string
* @param $code string
*/
function runkit_function_add($funcname, $arglist, $code);
/**
* Copy a function to a new function name
* @return bool
* @version PECL
* @param $funcname string
* @param $targetname string
*/
function runkit_function_copy($funcname, $targetname);
/**
* Replace a function definition with a new implementation
* @return bool
* @version PECL
* @param $funcname string
* @param $arglist string
* @param $code string
*/
function runkit_function_redefine($funcname, $arglist, $code);
/**
* Remove a function definition
* @return bool
* @version PECL
* @param $funcname string
*/
function runkit_function_remove($funcname);
/**
* Change a function's name
* @return bool
* @version PECL
* @param $funcname string
* @param $newname string
*/
function runkit_function_rename($funcname, $newname);
/**
* Process a PHP file importing function and class definitions, overwriting where appropriate
* @return bool
* @version PECL
* @param $filename string
* @param $flags (optional) int
*/
function runkit_import($filename, $flags);
/**
* Check the PHP syntax of the specified php code
* @return bool
* @version PECL
* @param $code string
*/
function runkit_lint($code);
/**
* Check the PHP syntax of the specified file
* @return bool
* @version PECL
* @param $filename string
*/
function runkit_lint_file($filename);
/**
* Dynamically adds a new method to a given class
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $args string
* @param $code string
* @param $flags (optional) int
*/
function runkit_method_add($classname, $methodname, $args, $code, $flags);
/**
* Copies a method from class to another
* @return bool
* @version PECL
* @param $dClass string
* @param $dMethod string
* @param $sClass string
* @param $sMethod (optional) string
*/
function runkit_method_copy($dClass, $dMethod, $sClass, $sMethod);
/**
* Dynamically changes the code of the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $args string
* @param $code string
* @param $flags (optional) int
*/
function runkit_method_redefine($classname, $methodname, $args, $code, $flags);
/**
* Dynamically removes the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
*/
function runkit_method_remove($classname, $methodname);
/**
* Dynamically changes the name of the given method
* @return bool
* @version PECL
* @param $classname string
* @param $methodname string
* @param $newname string
*/
function runkit_method_rename($classname, $methodname, $newname);
/**
* Specify a function to capture and/or process output from a runkit sandbox
* @return mixed
* @version PECL
* @param $sandbox object
* @param $callback (optional) mixed
*/
function runkit_sandbox_output_handler($sandbox, $callback);
/**
* Return numerically indexed array of registered superglobals
* @return array
* @version PECL
*/
function runkit_superglobals();
/**
* See if an exception was caught from the previous function
* @return bool
* @version 4.0.3 - 4.1.2 only, PECL
*/
function satellite_caught_exception();
/**
* Get the repository id for the latest exception.
* @return string
* @version 4.0.3 - 4.1.2 only, PECL
*/
function satellite_exception_id();
/**
* Get the exception struct for the latest exception
* @return OrbitStruct
* @version 4.0.3 - 4.1.2 only, PECL
*/
function satellite_exception_value();
/**
* NOT IMPLEMENTED
* @return int
* @version 4.0.3 - 4.1.2 only, PECL
* @param $obj object
*/
function satellite_get_repository_id($obj);
/**
* Instruct the type manager to load an IDL file
* @return bool
* @version 4.0.3 - 4.1.2 only, PECL
* @param $file string
*/
function satellite_load_idl($file);
/**
* Convert an object to its string representation
* @return string
* @version 4.1.0 - 4.1.2 only, PECL
* @param $obj object
*/
function satellite_object_to_string($obj);
/**
* List files and directories inside the specified path
* @return array
* @version PHP 5
* @param $directory string
* @param $sorting_order (optional) int
* @param $context (optional) resource
*/
function scandir($directory, $sorting_order, $context);
/**
* Acquire a semaphore
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $sem_identifier resource
*/
function sem_acquire($sem_identifier);
/**
* Get a semaphore id
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $key int
* @param $max_acquire (optional) int
* @param $perm (optional) int
* @param $auto_release (optional) int
*/
function sem_get($key, $max_acquire, $perm, $auto_release);
/**
* Release a semaphore
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $sem_identifier resource
*/
function sem_release($sem_identifier);
/**
* Remove a semaphore
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $sem_identifier resource
*/
function sem_remove($sem_identifier);
/**
* Generates a storable representation of a value
* @return string
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
* @param $value mixed
*/
function serialize($value);
/**
* Get number of rows affected by an immediate query
* @return int
* @version PHP 3 CVS only
* @param $result_id string
*/
function sesam_affected_rows($result_id);
/**
* Commit pending updates to the SESAM database
* @return bool
* @version PHP 3 CVS only
*/
function sesam_commit();
/**
* Open SESAM database connection
* @return bool
* @version PHP 3 CVS only
* @param $catalog string
* @param $schema string
* @param $user string
*/
function sesam_connect($catalog, $schema, $user);
/**
* Return status information for last SESAM call
* @return array
* @version PHP 3 CVS only
*/
function sesam_diagnostic();
/**
* Detach from SESAM connection
* @return bool
* @version PHP 3 CVS only
*/
function sesam_disconnect();
/**
* Returns error message of last SESAM call
* @return string
* @version PHP 3 CVS only
*/
function sesam_errormsg();
/**
* Execute an "immediate" SQL-statement
* @return string
* @version PHP 3 CVS only
* @param $query string
*/
function sesam_execimm($query);
/**
* Fetch one row as an associative array
* @return array
* @version PHP 3 CVS only
* @param $result_id string
* @param $whence (optional) int
* @param $offset (optional) int
*/
function sesam_fetch_array($result_id, $whence, $offset);
/**
* Return all or part of a query result
* @return mixed
* @version PHP 3 CVS only
* @param $result_id string
* @param $max_rows (optional) int
*/
function sesam_fetch_result($result_id, $max_rows);
/**
* Fetch one row as an array
* @return array
* @version PHP 3 CVS only
* @param $result_id string
* @param $whence (optional) int
* @param $offset (optional) int
*/
function sesam_fetch_row($result_id, $whence, $offset);
/**
* Return meta information about individual columns in a result
* @return array
* @version PHP 3 CVS only
* @param $result_id string
*/
function sesam_field_array($result_id);
/**
* Return one column name of the result set
* @return int
* @version PHP 3 CVS only
* @param $result_id string
* @param $index int
*/
function sesam_field_name($result_id, $index);
/**
* Releases resources for the query
* @return int
* @version PHP 3 CVS only
* @param $result_id string
*/
function sesam_free_result($result_id);
/**
* Return the number of fields/columns in a result set
* @return int
* @version PHP 3 CVS only
* @param $result_id string
*/
function sesam_num_fields($result_id);
/**
* Perform a SESAM SQL query and prepare the result
* @return string
* @version PHP 3 CVS only
* @param $query string
* @param $scrollable (optional) bool
*/
function sesam_query($query, $scrollable);
/**
* Discard any pending updates to the SESAM database
* @return bool
* @version PHP 3 CVS only
*/
function sesam_rollback();
/**
* Set scrollable cursor mode for subsequent fetches
* @return bool
* @version PHP 3 CVS only
* @param $result_id string
* @param $whence int
* @param $offset (optional) int
*/
function sesam_seek_row($result_id, $whence, $offset);
/**
* Set SESAM transaction parameters
* @return bool
* @version PHP 3 CVS only
* @param $isolation_level int
* @param $read_only int
*/
function sesam_settransaction($isolation_level, $read_only);
/**
* Return current cache expire
* @return int
* @version PHP 4 >= 4.2.0, PHP 5
* @param $new_cache_expire int
*/
function session_cache_expire($new_cache_expire);
/**
* Get and/or set the current cache limiter
* @return string
* @version PHP 4 >= 4.0.3, PHP 5
* @param $cache_limiter string
*/
function session_cache_limiter($cache_limiter);
/**
* Alias of session_write_close()
* @return &#13;
* @version PHP 4 >= 4.3.11, PHP 5
*/
function session_commit();
/**
* Decodes session data from a string
* @return bool
* @version PHP 4, PHP 5
* @param $data string
*/
function session_decode($data);
/**
* Destroys all data registered to a session
* @return bool
* @version PHP 4, PHP 5
*/
function session_destroy();
/**
* Encodes the current session data as a string
* @return string
* @version PHP 4, PHP 5
*/
function session_encode();
/**
* Get the session cookie parameters
* @return array
* @version PHP 4, PHP 5
*/
function session_get_cookie_params();
/**
* Get and/or set the current session id
* @return string
* @version PHP 4, PHP 5
* @param $id string
*/
function session_id($id);
/**
* Find out whether a global variable is registered in a session
* @return bool
* @version PHP 4, PHP 5
* @param $name string
*/
function session_is_registered($name);
/**
* Get and/or set the current session module
* @return string
* @version PHP 4, PHP 5
* @param $module string
*/
function session_module_name($module);
/**
* Get and/or set the current session name
* @return string
* @version PHP 4, PHP 5
* @param $name string
*/
function session_name($name);
/**
* Increments error counts and sets last error message
* @return bool
* @version PECL
* @param $error_level int
* @param $error_message (optional) string
*/
function session_pgsql_add_error($error_level, $error_message);
/**
* Returns number of errors and last error message
* @return array
* @version PECL
* @param $with_error_message bool
*/
function session_pgsql_get_error($with_error_message);
/**
* Get custom field value
* @return string
* @version PECL
*/
function session_pgsql_get_field();
/**
* Reset connection to session database servers
* @return bool
* @version PECL
*/
function session_pgsql_reset();
/**
* Set custom field value
* @return bool
* @version PECL
* @param $value string
*/
function session_pgsql_set_field($value);
/**
* Get current save handler status
* @return array
* @version PECL
*/
function session_pgsql_status();
/**
* Update the current session id with a newly generated one
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $delete_old_session bool
*/
function session_regenerate_id($delete_old_session);
/**
* Register one or more global variables with the current session
* @return bool
* @version PHP 4, PHP 5
* @param $name mixed
* @param $params1 (optional) mixed
*/
function session_register($name, $params1);
/**
* Get and/or set the current session save path
* @return string
* @version PHP 4, PHP 5
* @param $path string
*/
function session_save_path($path);
/**
* Set the session cookie parameters
* @return 
* @version PHP 4, PHP 5
* @param $lifetime int
* @param $path (optional) string
* @param $domain (optional) string
* @param $secure (optional) bool
*/
function session_set_cookie_params($lifetime, $path, $domain, $secure);
/**
* Sets user-level session storage functions
* @return bool
* @version PHP 4, PHP 5
* @param $open callback
* @param $close callback
* @param $read callback
* @param $write callback
* @param $destroy callback
* @param $gc callback
*/
function session_set_save_handler($open, $close, $read, $write, $destroy, $gc);
/**
* Initialize session data
* @return bool
* @version PHP 4, PHP 5
*/
function session_start();
/**
* Unregister a global variable from the current session
* @return bool
* @version PHP 4, PHP 5
* @param $name string
*/
function session_unregister($name);
/**
* Free all session variables
* @return 
* @version PHP 4, PHP 5
*/
function session_unset();
/**
* Write session data and end session
* @return 
* @version PHP 4 >= 4.0.4, PHP 5
*/
function session_write_close();
/**
* Send a cookie
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $name string
* @param $value (optional) string
* @param $expire (optional) int
* @param $path (optional) string
* @param $domain (optional) string
* @param $secure (optional) bool
*/
function setcookie($name, $value, $expire, $path, $domain, $secure);
/**
* Set locale information
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $category int
* @param $locale string
* @param $params1 (optional) string
*/
function setlocale($category, $locale, $params1);
/**
* Send a cookie without urlencoding the cookie value
* @return bool
* @version PHP 5
* @param $name string
* @param $value (optional) string
* @param $expire (optional) int
* @param $path (optional) string
* @param $domain (optional) string
* @param $secure (optional) bool
*/
function setrawcookie($name, $value, $expire, $path, $domain, $secure);
/**
* Set the type of a variable
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$var mixed
* @param $type string
*/
function settype(&$var, $type);
/**
* Sets a user-defined error handler function
* @return mixed
* @version PHP 4 >= 4.0.1, PHP 5
* @param $error_handler callback
* @param $error_types (optional) int
*/
function set_error_handler($error_handler, $error_types);
/**
* Sets a user-defined exception handler function
* @return string
* @version PHP 5
* @param $exception_handler callback
*/
function set_exception_handler($exception_handler);
/**
* Alias of stream_set_write_buffer()
* @return &#13;
* @version PHP 3 >= 3.0.8, PHP 4 >= 4.0.1, PHP 5
*/
function set_file_buffer();
/**
* Sets the include_path configuration option
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $new_include_path string
*/
function set_include_path($new_include_path);
/**
* Sets the current active configuration setting of magic_quotes_runtime
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $new_setting int
*/
function set_magic_quotes_runtime($new_setting);
/**
* Limits the maximum execution time
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $seconds int
*/
function set_time_limit($seconds);
/**
* Calculate the sha1 hash of a string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $str string
* @param $raw_output (optional) bool
*/
function sha1($str, $raw_output);
/**
* Calculate the sha1 hash of a file
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $filename string
* @param $raw_output (optional) bool
*/
function sha1_file($filename, $raw_output);
/**
* Execute command via shell and return the complete output as a string
* @return string
* @version PHP 4, PHP 5
* @param $cmd string
*/
function shell_exec($cmd);
/**
* Close shared memory block
* @return 
* @version PHP 4 >= 4.0.4, PHP 5
* @param $shmid int
*/
function shmop_close($shmid);
/**
* Delete shared memory block
* @return bool
* @version PHP 4 >= 4.0.4, PHP 5
* @param $shmid int
*/
function shmop_delete($shmid);
/**
* Create or open shared memory block
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $key int
* @param $flags string
* @param $mode int
* @param $size int
*/
function shmop_open($key, $flags, $mode, $size);
/**
* Read data from shared memory block
* @return string
* @version PHP 4 >= 4.0.4, PHP 5
* @param $shmid int
* @param $start int
* @param $count int
*/
function shmop_read($shmid, $start, $count);
/**
* Get size of shared memory block
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $shmid int
*/
function shmop_size($shmid);
/**
* Write data into shared memory block
* @return int
* @version PHP 4 >= 4.0.4, PHP 5
* @param $shmid int
* @param $data string
* @param $offset int
*/
function shmop_write($shmid, $data, $offset);
/**
* Creates or open a shared memory segment
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $key int
* @param $memsize (optional) int
* @param $perm (optional) int
*/
function shm_attach($key, $memsize, $perm);
/**
* Disconnects from shared memory segment
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $shm_identifier int
*/
function shm_detach($shm_identifier);
/**
* Returns a variable from shared memory
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $shm_identifier int
* @param $variable_key int
*/
function shm_get_var($shm_identifier, $variable_key);
/**
* Inserts or updates a variable in shared memory
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $shm_identifier int
* @param $variable_key int
* @param $variable mixed
*/
function shm_put_var($shm_identifier, $variable_key, $variable);
/**
* Removes shared memory from Unix systems
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $shm_identifier int
*/
function shm_remove($shm_identifier);
/**
* Removes a variable from shared memory
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $shm_identifier int
* @param $variable_key int
*/
function shm_remove_var($shm_identifier, $variable_key);
/**
* Alias of highlight_file()
* @return &#13;
* @version PHP 4, PHP 5
*/
function show_source();
/**
* Shuffle an array
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param &$array array
*/
function shuffle(&$array);
/**
* Calculate the similarity between two strings
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $first string
* @param $second string
* @param &$percent (optional) float
*/
function similar_text($first, $second, &$percent);
/**
* Get a SimpleXMLElement object from a DOM node.
* @return SimpleXMLElement
* @version PHP 5
* @param $node DOMNode
* @param $class_name (optional) string
*/
function simplexml_import_dom($node, $class_name);
/**
* Interprets an XML file into an object
* @return object
* @version PHP 5
* @param $filename string
* @param $class_name (optional) string
* @param $options (optional) int
* @param $ns (optional) string
* @param $is_prefix (optional) bool
*/
function simplexml_load_file($filename, $class_name, $options, $ns, $is_prefix);
/**
* Interprets a string of XML into an object
* @return object
* @version PHP 5
* @param $data string
* @param $class_name (optional) string
* @param $options (optional) int
* @param $ns (optional) string
* @param $is_prefix (optional) bool
*/
function simplexml_load_string($data, $class_name, $options, $ns, $is_prefix);
/**
* Sine
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function sin($arg);
/**
* Hyperbolic sine
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function sinh($arg);
/**
* Alias of count()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function sizeof();
/**
* Delay execution
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $seconds int
*/
function sleep($seconds);
/**
* Fetch an SNMP object
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
* @param $community string
* @param $object_id string
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmpget($hostname, $community, $object_id, $timeout, $retries);
/**
* Fetch a SNMP object
* @return string
* @version PHP 5
* @param $host string
* @param $community string
* @param $object_id string
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmpgetnext($host, $community, $object_id, $timeout, $retries);
/**
* Return all objects including their respective object ID within the specified one
* @return array
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $host string
* @param $community string
* @param $object_id string
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmprealwalk($host, $community, $object_id, $timeout, $retries);
/**
* Set an SNMP object
* @return bool
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $hostname string
* @param $community string
* @param $object_id string
* @param $type string
* @param $value mixed
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmpset($hostname, $community, $object_id, $type, $value, $timeout, $retries);
/**
* Fetch all the SNMP objects from an agent
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $hostname string
* @param $community string
* @param $object_id string
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmpwalk($hostname, $community, $object_id, $timeout, $retries);
/**
* Query for a tree of information about a network entity
* @return array
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $hostname string
* @param $community string
* @param $object_id string
* @param $timeout (optional) int
* @param $retries (optional) int
*/
function snmpwalkoid($hostname, $community, $object_id, $timeout, $retries);
/**
* Fetches the current value of the UCD library's quick_print setting
* @return bool
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
*/
function snmp_get_quick_print();
/**
* Return the method how the SNMP values will be returned
* @return int
* @version PHP 4 >= 4.3.3, PHP 5
*/
function snmp_get_valueretrieval();
/**
* Reads and parses a MIB file into the active MIB tree
* @return bool
* @version PHP 5
* @param $filename string
*/
function snmp_read_mib($filename);
/**
* Return all values that are enums with their enum value instead of the raw integer
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
* @param $enum_print int
*/
function snmp_set_enum_print($enum_print);
/**
* Return all objects including their respective object id within the specified one
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
* @param $oid_numeric_print int
*/
function snmp_set_oid_numeric_print($oid_numeric_print);
/**
* Set the value of quick_print within the UCD SNMP library
* @return 
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $quick_print bool
*/
function snmp_set_quick_print($quick_print);
/**
* Specify the method how the SNMP values will be returned
* @return 
* @version PHP 4 >= 4.3.3, PHP 5
* @param $method int
*/
function snmp_set_valueretrieval($method);
/**
* Accepts a connection on a socket
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
*/
function socket_accept($socket);
/**
* Binds a name to a socket
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $address string
* @param $port (optional) int
*/
function socket_bind($socket, $address, $port);
/**
* Clears the error on the socket or the last error code
* @return 
* @version PHP 4 >= 4.2.0, PHP 5
* @param $socket resource
*/
function socket_clear_error($socket);
/**
* Closes a socket resource
* @return 
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
*/
function socket_close($socket);
/**
* Initiates a connection on a socket
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $address string
* @param $port (optional) int
*/
function socket_connect($socket, $address, $port);
/**
* Create a socket (endpoint for communication)
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $domain int
* @param $type int
* @param $protocol int
*/
function socket_create($domain, $type, $protocol);
/**
* Opens a socket on port to accept connections
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
* @param $port int
* @param $backlog (optional) int
*/
function socket_create_listen($port, $backlog);
/**
* Creates a pair of indistinguishable sockets and stores them in an array
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $domain int
* @param $type int
* @param $protocol int
* @param &$fd array
*/
function socket_create_pair($domain, $type, $protocol, &$fd);
/**
* Queries the remote side of the given socket which may either result in host/port or in a Unix filesystem path, dependent on its type
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param &$addr string
* @param &$port (optional) int
*/
function socket_getpeername($socket, &$addr, &$port);
/**
* Queries the local side of the given socket which may either result in host/port or in a Unix filesystem path, dependent on its type
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param &$addr string
* @param &$port (optional) int
*/
function socket_getsockname($socket, &$addr, &$port);
/**
* Gets socket options for the socket
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $socket resource
* @param $level int
* @param $optname int
*/
function socket_get_option($socket, $level, $optname);
/**
* Alias of stream_get_meta_data()
* @return &#13;
* @version PHP 4, PHP 5
*/
function socket_get_status();
/**
* Returns the last error on the socket
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
*/
function socket_last_error($socket);
/**
* Listens for a connection on a socket
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $backlog (optional) int
*/
function socket_listen($socket, $backlog);
/**
* Reads a maximum of length bytes from a socket
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $length int
* @param $type (optional) int
*/
function socket_read($socket, $length, $type);
/**
* Receives data from a connected socket
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param &$buf string
* @param $len int
* @param $flags int
*/
function socket_recv($socket, &$buf, $len, $flags);
/**
* Receives data from a socket, connected or not
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param &$buf string
* @param $len int
* @param $flags int
* @param &$name string
* @param &$port (optional) int
*/
function socket_recvfrom($socket, &$buf, $len, $flags, &$name, &$port);
/**
* Runs the select() system call on the given arrays of sockets with a specified timeout
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param &$read array
* @param &$write array
* @param &$except array
* @param $tv_sec int
* @param $tv_usec (optional) int
*/
function socket_select(&$read, &$write, &$except, $tv_sec, $tv_usec);
/**
* Sends data to a connected socket
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $buf string
* @param $len int
* @param $flags int
*/
function socket_send($socket, $buf, $len, $flags);
/**
* Sends a message to a socket, whether it is connected or not
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $buf string
* @param $len int
* @param $flags int
* @param $addr string
* @param $port (optional) int
*/
function socket_sendto($socket, $buf, $len, $flags, $addr, $port);
/**
* Sets blocking mode on a socket resource
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5
* @param $socket resource
*/
function socket_set_block($socket);
/**
* Alias of stream_set_blocking()
* @return &#13;
* @version PHP 4, PHP 5
*/
function socket_set_blocking();
/**
* Sets nonblocking mode for file descriptor fd
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
*/
function socket_set_nonblock($socket);
/**
* Sets socket options for the socket
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $socket resource
* @param $level int
* @param $optname int
* @param $optval mixed
*/
function socket_set_option($socket, $level, $optname, $optval);
/**
* Alias of stream_set_timeout()
* @return &#13;
* @version PHP 4, PHP 5
*/
function socket_set_timeout();
/**
* Shuts down a socket for receiving, sending, or both
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $how (optional) int
*/
function socket_shutdown($socket, $how);
/**
* Return a string describing a socket error
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $errno int
*/
function socket_strerror($errno);
/**
* Write to a socket
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $socket resource
* @param $buffer string
* @param $length (optional) int
*/
function socket_write($socket, $buffer, $length);
/**
* Sort an array
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param &$array array
* @param $sort_flags (optional) int
*/
function sort(&$array, $sort_flags);
/**
* Calculate the soundex key of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function soundex($str);
/**
* Split string into array by regular expression
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $pattern string
* @param $string string
* @param $limit (optional) int
*/
function split($pattern, $string, $limit);
/**
* Split string into array by regular expression case insensitive
* @return array
* @version PHP 4 >= 4.0.1, PHP 5
* @param $pattern string
* @param $string string
* @param $limit (optional) int
*/
function spliti($pattern, $string, $limit);
/**
* Default implementation for __autoload()
* @return 
* @version PHP 5 >= 5.1.0RC1
* @param $class_name string
* @param $file_extensions (optional) string
*/
function spl_autoload($class_name, $file_extensions);
/**
* Try all registered __autoload() function to load the requested class
* @return 
* @version PHP 5 >= 5.1.0RC1
* @param $class_name string
*/
function spl_autoload_call($class_name);
/**
* Register and return default file extensions for spl_autoload
* @return string
* @version PHP 5 >= 5.1.0RC1
* @param $file_extensions string
*/
function spl_autoload_extensions($file_extensions);
/**
* Return all registered __autoload() functionns
* @return array
* @version PHP 5 >= 5.1.0RC1
*/
function spl_autoload_functions();
/**
* Register given function as __autoload() implementation
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $autoload_function mixed
*/
function spl_autoload_register($autoload_function);
/**
* Unregister given function as __autoload() implementation
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $autoload_function mixed
*/
function spl_autoload_unregister($autoload_function);
/**
* Return available SPL classes
* @return array
* @version PHP 5
*/
function spl_classes();
/**
* Return a formatted string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $args (optional) mixed
* @param $params1 (optional) mixed
*/
function sprintf($format, $args, $params1);
/**
* 
* @return SQLiteDatabase->arrayQuery
* @version PHP 5
*/
function sqlite_array_query();
/**
* 
* @return SQLiteDatabase->busyTimeout
* @version PHP 5
*/
function sqlite_busy_timeout();
/**
* 
* @return SQLiteDatabase->changes
* @version PHP 5
*/
function sqlite_changes();
/**
* Closes an open SQLite database
* @return 
* @version PHP 5
* @param $dbhandle resource
*/
function sqlite_close($dbhandle);
/**
* 
* @return SQLiteResult->column
* @version PHP 5
*/
function sqlite_column();
/**
* 
* @return SQLiteDatabase->createAggregate
* @version PHP 5
*/
function sqlite_create_aggregate();
/**
* 
* @return SQLiteDatabase->createFunction
* @version PHP 5
*/
function sqlite_create_function();
/**
* 
* @return SQLiteResult->current
* @version PHP 5
*/
function sqlite_current();
/**
* Returns the textual description of an error code
* @return string
* @version PHP 5
* @param $error_code int
*/
function sqlite_error_string($error_code);
/**
* Escapes a string for use as a query parameter
* @return string
* @version PHP 5
* @param $item string
*/
function sqlite_escape_string($item);
/**
* 
* @return SQLiteDatabase->exec
* @version PHP 5
*/
function sqlite_exec();
/**
* Opens a SQLite database and returns a SQLiteDatabase object
* @return SQLiteDatabase
* @version PHP 5
* @param $filename string
* @param $mode (optional) int
* @param &$error_message (optional) string
*/
function sqlite_factory($filename, $mode, &$error_message);
/**
* 
* @return SQLiteResult->fetchAll
* @version PHP 5
*/
function sqlite_fetch_all();
/**
* 
* @return SQLiteResult->fetch
* @version PHP 5
*/
function sqlite_fetch_array();
/**
* 
* @return SQLiteDatabase->fetchColumnTypes
* @version PHP 5
*/
function sqlite_fetch_column_types();
/**
* 
* @return SQLiteResult->fetchObject
* @version PHP 5
*/
function sqlite_fetch_object();
/**
* 
* @return SQLiteResult->fetchSingle
* @version PHP 5
*/
function sqlite_fetch_single();
/**
* Alias of sqlite_fetch_single()
* @return &#13;
* @version PHP 5
*/
function sqlite_fetch_string();
/**
* 
* @return SQLiteResult->fieldName
* @version PHP 5
*/
function sqlite_field_name();
/**
* Finds whether or not more rows are available
* @return bool
* @version PHP 5
* @param $result resource
*/
function sqlite_has_more($result);
/**
* 
* @return SQLiteResult->hasPrev
* @version PHP 5
*/
function sqlite_has_prev();
/**
* 
* @return SQLiteDatabase->lastError
* @version PHP 5
*/
function sqlite_last_error();
/**
* 
* @return SQLiteDatabase->lastInsertRowid
* @version PHP 5
*/
function sqlite_last_insert_rowid();
/**
* Returns the encoding of the linked SQLite library
* @return string
* @version PHP 5
*/
function sqlite_libencoding();
/**
* Returns the version of the linked SQLite library
* @return string
* @version PHP 5
*/
function sqlite_libversion();
/**
* 
* @return SQLiteResult->next
* @version PHP 5
*/
function sqlite_next();
/**
* 
* @return SQLiteResult->numFields
* @version PHP 5
*/
function sqlite_num_fields();
/**
* 
* @return SQLiteResult->numRows
* @version PHP 5
*/
function sqlite_num_rows();
/**
* Opens a SQLite database and create the database if it does not exist
* @return resource
* @version PHP 5
* @param $filename string
* @param $mode (optional) int
* @param &$error_message (optional) string
*/
function sqlite_open($filename, $mode, &$error_message);
/**
* Opens a persistent handle to an SQLite database and create the database if it does not exist
* @return resource
* @version PHP 5
* @param $filename string
* @param $mode (optional) int
* @param &$error_message (optional) string
*/
function sqlite_popen($filename, $mode, &$error_message);
/**
* 
* @return SQLiteResult->prev
* @version PHP 5
*/
function sqlite_prev();
/**
* 
* @return SQLiteDatabase->query
* @version PHP 5
*/
function sqlite_query();
/**
* 
* @return SQLiteResult->rewind
* @version PHP 5
*/
function sqlite_rewind();
/**
* 
* @return SQLiteResult->seek
* @version PHP 5
*/
function sqlite_seek();
/**
* 
* @return SQLiteDatabase->singleQuery
* @version PHP 5
*/
function sqlite_single_query();
/**
* Decode binary data passed as parameters to an UDF
* @return string
* @version PHP 5
* @param $data string
*/
function sqlite_udf_decode_binary($data);
/**
* Encode binary data before returning it from an UDF
* @return string
* @version PHP 5
* @param $data string
*/
function sqlite_udf_encode_binary($data);
/**
* 
* @return SQLiteDatabase->unbufferedQuery
* @version PHP 5
*/
function sqlite_unbuffered_query();
/**
* 
* @return SQLiteResult->valid
* @version PHP 5
*/
function sqlite_valid();
/**
* Make regular expression for case insensitive match
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function sql_regcase($string);
/**
* Square root
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function sqrt($arg);
/**
* Seed the random number generator
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $seed int
*/
function srand($seed);
/**
* Parses input from a string according to a format
* @return mixed
* @version PHP 4 >= 4.0.1, PHP 5
* @param $str string
* @param $format string
* @param &$... (optional) mixed
*/
function sscanf($str, $format, &$...);
/**
* Authenticate using a public hostkey
* @return bool
* @version PECL
* @param $session resource
* @param $username string
* @param $hostname string
* @param $pubkeyfile string
* @param $privkeyfile string
* @param $passphrase (optional) string
* @param $local_username (optional) string
*/
function ssh2_auth_hostbased_file($session, $username, $hostname, $pubkeyfile, $privkeyfile, $passphrase, $local_username);
/**
* Authenticate as "none"
* @return mixed
* @version PECL
* @param $session resource
* @param $username string
*/
function ssh2_auth_none($session, $username);
/**
* Authenticate over SSH using a plain password
* @return bool
* @version PECL
* @param $session resource
* @param $username string
* @param $password string
*/
function ssh2_auth_password($session, $username, $password);
/**
* Authenticate using a public key
* @return bool
* @version PECL
* @param $session resource
* @param $username string
* @param $pubkeyfile string
* @param $privkeyfile string
* @param $passphrase (optional) string
*/
function ssh2_auth_pubkey_file($session, $username, $pubkeyfile, $privkeyfile, $passphrase);
/**
* Connect to an SSH server
* @return resource
* @version PECL
* @param $host string
* @param $port (optional) int
* @param $methods (optional) array
* @param $callbacks (optional) array
*/
function ssh2_connect($host, $port, $methods, $callbacks);
/**
* Execute a command on a remote server
* @return resource
* @version PECL
* @param $session resource
* @param $command string
* @param $pty (optional) string
* @param $env (optional) array
* @param $width (optional) int
* @param $height (optional) int
* @param $width_height_type (optional) int
*/
function ssh2_exec($session, $command, $pty, $env, $width, $height, $width_height_type);
/**
* Fetch an extended data stream
* @return resource
* @version PECL
* @param $channel resource
* @param $streamid int
*/
function ssh2_fetch_stream($channel, $streamid);
/**
* Retreive fingerprint of remote server
* @return string
* @version PECL
* @param $session resource
* @param $flags (optional) int
*/
function ssh2_fingerprint($session, $flags);
/**
* Return list of negotiated methods
* @return array
* @version PECL
* @param $session resource
*/
function ssh2_methods_negotiated($session);
/**
* Add an authorized publickey
* @return bool
* @version PECL
* @param $pkey resource
* @param $algoname string
* @param $blob string
* @param $overwrite (optional) bool
* @param $attributes (optional) array
*/
function ssh2_publickey_add($pkey, $algoname, $blob, $overwrite, $attributes);
/**
* Initialize Publickey subsystem
* @return resource
* @version PECL
* @param $session resource
*/
function ssh2_publickey_init($session);
/**
* List currently authorized publickeys
* @return array
* @version PECL
* @param $pkey resource
*/
function ssh2_publickey_list($pkey);
/**
* Remove an authorized publickey
* @return bool
* @version PECL
* @param $pkey resource
* @param $algoname string
* @param $blob string
*/
function ssh2_publickey_remove($pkey, $algoname, $blob);
/**
* Request a file via SCP
* @return bool
* @version PECL
* @param $session resource
* @param $remote_file string
* @param $local_file string
*/
function ssh2_scp_recv($session, $remote_file, $local_file);
/**
* Send a file via SCP
* @return bool
* @version PECL
* @param $session resource
* @param $local_file string
* @param $remote_file string
* @param $create_mode (optional) int
*/
function ssh2_scp_send($session, $local_file, $remote_file, $create_mode);
/**
* Initialize SFTP subsystem
* @return resource
* @version PECL
* @param $session resource
*/
function ssh2_sftp($session);
/**
* Stat a symbolic link
* @return array
* @version PECL
* @param $sftp resource
* @param $path string
*/
function ssh2_sftp_lstat($sftp, $path);
/**
* Create a directory
* @return bool
* @version PECL
* @param $sftp resource
* @param $dirname string
* @param $mode (optional) int
* @param $recursive (optional) bool
*/
function ssh2_sftp_mkdir($sftp, $dirname, $mode, $recursive);
/**
* Return the target of a symbolic link
* @return string
* @version PECL
* @param $sftp resource
* @param $link string
*/
function ssh2_sftp_readlink($sftp, $link);
/**
* Resolve the realpath of a provided path string
* @return string
* @version PECL
* @param $sftp resource
* @param $filename string
*/
function ssh2_sftp_realpath($sftp, $filename);
/**
* Rename a remote file
* @return bool
* @version PECL
* @param $sftp resource
* @param $from string
* @param $to string
*/
function ssh2_sftp_rename($sftp, $from, $to);
/**
* Remove a directory
* @return bool
* @version PECL
* @param $sftp resource
* @param $dirname string
*/
function ssh2_sftp_rmdir($sftp, $dirname);
/**
* Stat a file on a remote filesystem
* @return array
* @version PECL
* @param $sftp resource
* @param $path string
*/
function ssh2_sftp_stat($sftp, $path);
/**
* Create a symlink
* @return bool
* @version PECL
* @param $sftp resource
* @param $target string
* @param $link string
*/
function ssh2_sftp_symlink($sftp, $target, $link);
/**
* Delete a file
* @return bool
* @version PECL
* @param $sftp resource
* @param $filename string
*/
function ssh2_sftp_unlink($sftp, $filename);
/**
* Request an interactive shell
* @return resource
* @version PECL
* @param $session resource
* @param $term_type (optional) string
* @param $env (optional) array
* @param $width (optional) int
* @param $height (optional) int
* @param $width_height_type (optional) int
*/
function ssh2_shell($session, $term_type, $env, $width, $height, $width_height_type);
/**
* Open a tunnel through a remote server
* @return resource
* @version PECL
* @param $session resource
* @param $host string
* @param $port int
*/
function ssh2_tunnel($session, $host, $port);
/**
* Gives information about a file
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function stat($filename);
/**
* Returns the absolute deviation of an array of values
* @return float
* @version PECL
* @param $a array
*/
function stats_absolute_deviation($a);
/**
* CDF function for BETA Distribution. Calculates any one parameter of the beta distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_beta($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the binomial distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_binomial($par1, $par2, $par3, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_cauchy($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the chi-square distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $which int
*/
function stats_cdf_chisquare($par1, $par2, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $which int
*/
function stats_cdf_exponential($par1, $par2, $which);
/**
* Calculates any one parameter of the F distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_f($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the gamma distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_gamma($par1, $par2, $par3, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_laplace($par1, $par2, $par3, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_logistic($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the negative binomial distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_negative_binomial($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the non-central chi-square distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_noncentral_chisquare($par1, $par2, $par3, $which);
/**
* Calculates any one parameter of the Non-central F distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $par4 float
* @param $which int
*/
function stats_cdf_noncentral_f($par1, $par2, $par3, $par4, $which);
/**
* Calculates any one parameter of the Poisson distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $which int
*/
function stats_cdf_poisson($par1, $par2, $which);
/**
* Calculates any one parameter of the T distribution given values for the others.
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $which int
*/
function stats_cdf_t($par1, $par2, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_uniform($par1, $par2, $par3, $which);
/**
* Not documented
* @return float
* @version PECL
* @param $par1 float
* @param $par2 float
* @param $par3 float
* @param $which int
*/
function stats_cdf_weibull($par1, $par2, $par3, $which);
/**
* Computes the covariance of two data sets
* @return float
* @version PECL
* @param $a array
* @param $b array
*/
function stats_covariance($a, $b);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $a float
* @param $b float
*/
function stats_dens_beta($x, $a, $b);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $ave float
* @param $stdev float
*/
function stats_dens_cauchy($x, $ave, $stdev);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $dfr float
*/
function stats_dens_chisquare($x, $dfr);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $scale float
*/
function stats_dens_exponential($x, $scale);
/**
* --
* @return float
* @version PECL
* @param $x float
* @param $dfr1 float
* @param $dfr2 float
*/
function stats_dens_f($x, $dfr1, $dfr2);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $shape float
* @param $scale float
*/
function stats_dens_gamma($x, $shape, $scale);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $ave float
* @param $stdev float
*/
function stats_dens_laplace($x, $ave, $stdev);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $ave float
* @param $stdev float
*/
function stats_dens_logistic($x, $ave, $stdev);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $ave float
* @param $stdev float
*/
function stats_dens_normal($x, $ave, $stdev);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $n float
* @param $pi float
*/
function stats_dens_pmf_binomial($x, $n, $pi);
/**
* --
* @return float
* @version PECL
* @param $n1 float
* @param $n2 float
* @param $N1 float
* @param $N2 float
*/
function stats_dens_pmf_hypergeometric($n1, $n2, $N1, $N2);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $lb float
*/
function stats_dens_pmf_poisson($x, $lb);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $dfr float
*/
function stats_dens_t($x, $dfr);
/**
* Not documented
* @return float
* @version PECL
* @param $x float
* @param $a float
* @param $b float
*/
function stats_dens_weibull($x, $a, $b);
/**
* Returns the harmonic mean of an array of values
* @return number
* @version PECL
* @param $a array
*/
function stats_harmonic_mean($a);
/**
* Computes the kurtosis of the data in the array
* @return float
* @version PECL
* @param $a array
*/
function stats_kurtosis($a);
/**
* Generates beta random deviate
* @return float
* @version PECL
* @param $a float
* @param $b float
*/
function stats_rand_gen_beta($a, $b);
/**
* Generates random deviate from the distribution of a chisquare with "df" degrees of freedom random variable.
* @return float
* @version PECL
* @param $df float
*/
function stats_rand_gen_chisquare($df);
/**
* Generates a single random deviate from an exponential distribution with mean "av"
* @return float
* @version PECL
* @param $av float
*/
function stats_rand_gen_exponential($av);
/**
* Generates a random deviate from the F (variance ratio) distribution with "dfn" degrees of freedom in the numerator and "dfd" degrees of freedom in the denominator. Method : directly generates ratio of chisquare variates
* @return float
* @version PECL
* @param $dfn float
* @param $dfd float
*/
function stats_rand_gen_f($dfn, $dfd);
/**
* Generates uniform float between low (exclusive) and high (exclusive)
* @return float
* @version PECL
* @param $low float
* @param $high float
*/
function stats_rand_gen_funiform($low, $high);
/**
* Generates random deviates from a gamma distribution
* @return float
* @version PECL
* @param $a float
* @param $r float
*/
function stats_rand_gen_gamma($a, $r);
/**
* Generates a single random deviate from a Poisson distribution with mean "mu" (mu >= 0.0).
* @return int
* @version PECL
* @param $mu float
*/
function stats_rand_gen_ipoisson($mu);
/**
* Generates integer uniformly distributed between LOW (inclusive) and HIGH (inclusive)
* @return int
* @version PECL
* @param $low int
* @param $high int
*/
function stats_rand_gen_iuniform($low, $high);
/**
* Generates a single random deviate from a noncentral T distribution
* @return float
* @version PECL
* @param $df float
* @param $xnonc float
*/
function stats_rand_gen_noncentral_t($df, $xnonc);
/**
* Generates a single random deviate from a normal distribution with mean, av, and standard deviation, sd (sd >= 0). Method : Renames SNORM from TOMS as slightly modified by BWB to use RANF instead of SUNIF.
* @return float
* @version PECL
* @param $av float
* @param $sd float
*/
function stats_rand_gen_normal($av, $sd);
/**
* Generates a single random deviate from a T distribution
* @return float
* @version PECL
* @param $df float
*/
function stats_rand_gen_t($df);
/**
* generate two seeds for the RGN random number generator
* @return array
* @version PECL
* @param $phrase string
*/
function stats_rand_phrase_to_seeds($phrase);
/**
* Returns a random floating point number from a uniform distribution over 0 - 1 (endpoints of this interval are not returned) using the current generator
* @return float
* @version PECL
*/
function stats_rand_ranf();
/**
* Not documented
* @return 
* @version PECL
* @param $iseed1 int
* @param $iseed2 int
*/
function stats_rand_setall($iseed1, $iseed2);
/**
* Computes the skewness of the data in the array
* @return float
* @version PECL
* @param $a array
*/
function stats_skew($a);
/**
* Returns the standard deviation
* @return float
* @version PECL
* @param $a array
* @param $sample (optional) bool
*/
function stats_standard_deviation($a, $sample);
/**
* Not documented
* @return float
* @version PECL
* @param $x int
* @param $n int
*/
function stats_stat_binomial_coef($x, $n);
/**
* Not documented
* @return float
* @version PECL
* @param $arr1 array
* @param $arr2 array
*/
function stats_stat_correlation($arr1, $arr2);
/**
* Not documented
* @return float
* @version PECL
* @param $arr1 array
* @param $arr2 array
*/
function stats_stat_independent_t($arr1, $arr2);
/**
* --
* @return float
* @version PECL
* @param $arr1 array
* @param $arr2 array
*/
function stats_stat_innerproduct($arr1, $arr2);
/**
* Not documented
* @return float
* @version PECL
* @param $arr1 array
* @param $arr2 array
*/
function stats_stat_paired_t($arr1, $arr2);
/**
* Not documented
* @return float
* @version PECL
* @param $df float
* @param $xnonc float
*/
function stats_stat_percentile($df, $xnonc);
/**
* Not documented
* @return float
* @version PECL
* @param $arr array
* @param $power float
*/
function stats_stat_powersum($arr, $power);
/**
* Returns the population variance
* @return float
* @version PECL
* @param $a array
* @param $sample (optional) bool
*/
function stats_variance($a, $sample);
/**
* Binary safe case-insensitive string comparison
* @return int
* @version PHP 3 >= 3.0.2, PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
*/
function strcasecmp($str1, $str2);
/**
* Alias of strstr()
* @return &#13;
* @version PHP 3, PHP 4, PHP 5
*/
function strchr();
/**
* Binary safe string comparison
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
*/
function strcmp($str1, $str2);
/**
* Locale based string comparison
* @return int
* @version PHP 4 >= 4.0.5, PHP 5
* @param $str1 string
* @param $str2 string
*/
function strcoll($str1, $str2);
/**
* Find length of initial segment not matching mask
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
* @param $start (optional) int
* @param $length (optional) int
*/
function strcspn($str1, $str2, $start, $length);
/**
* Append bucket to brigade
* @return 
* @version PHP 5
* @param $brigade resource
* @param $bucket resource
*/
function stream_bucket_append($brigade, $bucket);
/**
* Return a bucket object from the brigade for operating on
* @return object
* @version PHP 5
* @param $brigade resource
*/
function stream_bucket_make_writeable($brigade);
/**
* Create a new bucket for use on the current stream
* @return object
* @version PHP 5
* @param $stream resource
* @param $buffer string
*/
function stream_bucket_new($stream, $buffer);
/**
* Prepend bucket to brigade
* @return 
* @version PHP 5
* @param $brigade resource
* @param $bucket resource
*/
function stream_bucket_prepend($brigade, $bucket);
/**
* Create a streams context
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $options array
* @param $params (optional) array
*/
function stream_context_create($options, $params);
/**
* Retreive the default streams context
* @return resource
* @version PHP 5 >= 5.1.0RC1
* @param $options array
*/
function stream_context_get_default($options);
/**
* Retrieve options for a stream/wrapper/context
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream_or_context resource
*/
function stream_context_get_options($stream_or_context);
/**
* Sets an option for a stream/wrapper/context
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream_or_context resource
* @param $wrapper string
* @param $option string
* @param $value mixed
*/
function stream_context_set_option($stream_or_context, $wrapper, $option, $value);
/**
* Set parameters for a stream/wrapper/context
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream_or_context resource
* @param $params array
*/
function stream_context_set_params($stream_or_context, $params);
/**
* Copies data from one stream to another
* @return int
* @version PHP 5
* @param $source resource
* @param $dest resource
* @param $maxlength (optional) int
* @param $offset (optional) int
*/
function stream_copy_to_stream($source, $dest, $maxlength, $offset);
/**
* Attach a filter to a stream
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
* @param $filtername string
* @param $read_write (optional) int
* @param $params (optional) mixed
*/
function stream_filter_append($stream, $filtername, $read_write, $params);
/**
* Attach a filter to a stream
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
* @param $filtername string
* @param $read_write (optional) int
* @param $params (optional) mixed
*/
function stream_filter_prepend($stream, $filtername, $read_write, $params);
/**
* Register a stream filter implemented as a PHP class derived from php_user_filter
* @return bool
* @version PHP 5
* @param $filtername string
* @param $classname string
*/
function stream_filter_register($filtername, $classname);
/**
* Remove a filter from a stream
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $stream_filter resource
*/
function stream_filter_remove($stream_filter);
/**
* Reads remainder of a stream into a string
* @return string
* @version PHP 5
* @param $handle resource
* @param $maxlength (optional) int
* @param $offset (optional) int
*/
function stream_get_contents($handle, $maxlength, $offset);
/**
* Retrieve list of registered filters
* @return array
* @version PHP 5
*/
function stream_get_filters();
/**
* Gets line from stream resource up to a given delimiter
* @return string
* @version PHP 5
* @param $handle resource
* @param $length int
* @param $ending (optional) string
*/
function stream_get_line($handle, $length, $ending);
/**
* Retrieves header/meta data from streams/file pointers
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
*/
function stream_get_meta_data($stream);
/**
* Retrieve list of registered socket transports
* @return array
* @version PHP 5
*/
function stream_get_transports();
/**
* Retrieve list of registered streams
* @return array
* @version PHP 5
*/
function stream_get_wrappers();
/**
* Alias of stream_wrapper_register()
* @return &#13;
* @version PHP 4 >= 4.3.0, PHP 5
*/
function stream_register_wrapper();
/**
* Runs the equivalent of the select() system call on the given arrays of streams with a timeout specified by tv_sec and tv_usec
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param &$read array
* @param &$write array
* @param &$except array
* @param $tv_sec int
* @param $tv_usec (optional) int
*/
function stream_select(&$read, &$write, &$except, $tv_sec, $tv_usec);
/**
* Set blocking/non-blocking mode on a stream
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
* @param $mode int
*/
function stream_set_blocking($stream, $mode);
/**
* Set timeout period on a stream
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
* @param $seconds int
* @param $microseconds (optional) int
*/
function stream_set_timeout($stream, $seconds, $microseconds);
/**
* Sets file buffering on the given stream
* @return int
* @version PHP 4 >= 4.3.0, PHP 5
* @param $stream resource
* @param $buffer int
*/
function stream_set_write_buffer($stream, $buffer);
/**
* Accept a connection on a socket created by stream_socket_server()
* @return resource
* @version PHP 5
* @param $server_socket resource
* @param $timeout (optional) float
* @param &$peername (optional) string
*/
function stream_socket_accept($server_socket, $timeout, &$peername);
/**
* Open Internet or Unix domain socket connection
* @return resource
* @version PHP 5
* @param $remote_socket string
* @param &$errno (optional) int
* @param &$errstr (optional) string
* @param $timeout (optional) float
* @param $flags (optional) int
* @param $context (optional) resource
*/
function stream_socket_client($remote_socket, &$errno, &$errstr, $timeout, $flags, $context);
/**
* Turns encryption on/off on an already connected socket
* @return mixed
* @version PHP 5 >= 5.1.0RC1
* @param $stream resource
* @param $enable bool
* @param $crypto_type (optional) int
* @param $session_stream (optional) resource
*/
function stream_socket_enable_crypto($stream, $enable, $crypto_type, $session_stream);
/**
* Retrieve the name of the local or remote sockets
* @return string
* @version PHP 5
* @param $handle resource
* @param $want_peer bool
*/
function stream_socket_get_name($handle, $want_peer);
/**
* Creates a pair of connected, indistinguishable socket streams
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $domain int
* @param $type int
* @param $protocol int
*/
function stream_socket_pair($domain, $type, $protocol);
/**
* Receives data from a socket, connected or not
* @return string
* @version PHP 5
* @param $socket resource
* @param $length int
* @param $flags (optional) int
* @param &$address (optional) string
*/
function stream_socket_recvfrom($socket, $length, $flags, &$address);
/**
* Sends a message to a socket, whether it is connected or not
* @return int
* @version PHP 5
* @param $socket resource
* @param $data string
* @param $flags (optional) int
* @param $address (optional) string
*/
function stream_socket_sendto($socket, $data, $flags, $address);
/**
* Create an Internet or Unix domain server socket
* @return resource
* @version PHP 5
* @param $local_socket string
* @param &$errno (optional) int
* @param &$errstr (optional) string
* @param $flags (optional) int
* @param $context (optional) resource
*/
function stream_socket_server($local_socket, &$errno, &$errstr, $flags, $context);
/**
* Register a URL wrapper implemented as a PHP class
* @return bool
* @version PHP 4 >= 4.3.2, PHP 5
* @param $protocol string
* @param $classname string
*/
function stream_wrapper_register($protocol, $classname);
/**
* Restores a previously unregistered built-in wrapper
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $protocol string
*/
function stream_wrapper_restore($protocol);
/**
* Unregister a URL wrapper
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $protocol string
*/
function stream_wrapper_unregister($protocol);
/**
* Format a local time/date according to locale settings
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $timestamp (optional) int
*/
function strftime($format, $timestamp);
/**
* Un-quote string quoted with addcslashes()
* @return string
* @version PHP 4, PHP 5
* @param $str string
*/
function stripcslashes($str);
/**
* Find position of first occurrence of a case-insensitive string
* @return int
* @version PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
*/
function stripos($haystack, $needle, $offset);
/**
* Un-quote string quoted with addslashes()
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function stripslashes($str);
/**
* Strip HTML and PHP tags from a string
* @return string
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $str string
* @param $allowable_tags (optional) string
*/
function strip_tags($str, $allowable_tags);
/**
* Case-insensitive strstr()
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $haystack string
* @param $needle string
*/
function stristr($haystack, $needle);
/**
* Get string length
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function strlen($string);
/**
* Case insensitive string comparisons using a "natural order" algorithm
* @return int
* @version PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
*/
function strnatcasecmp($str1, $str2);
/**
* String comparisons using a "natural order" algorithm
* @return int
* @version PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
*/
function strnatcmp($str1, $str2);
/**
* Binary safe case-insensitive string comparison of the first n characters
* @return int
* @version PHP 4 >= 4.0.2, PHP 5
* @param $str1 string
* @param $str2 string
* @param $len int
*/
function strncasecmp($str1, $str2, $len);
/**
* Binary safe string comparison of the first n characters
* @return int
* @version PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
* @param $len int
*/
function strncmp($str1, $str2, $len);
/**
* Search a string for any of a set of characters
* @return string
* @version PHP 5
* @param $haystack string
* @param $char_list string
*/
function strpbrk($haystack, $char_list);
/**
* Find position of first occurrence of a string
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $haystack string
* @param $needle mixed
* @param $offset (optional) int
*/
function strpos($haystack, $needle, $offset);
/**
* Parse a time/date generated with strftime()
* @return array
* @version PHP 5 >= 5.1.0RC1
* @param $date string
* @param $format string
*/
function strptime($date, $format);
/**
* Find the last occurrence of a character in a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $haystack string
* @param $needle string
*/
function strrchr($haystack, $needle);
/**
* Reverse a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function strrev($string);
/**
* Find position of last occurrence of a case-insensitive string in a string
* @return int
* @version PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
*/
function strripos($haystack, $needle, $offset);
/**
* Find position of last occurrence of a char in a string
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
*/
function strrpos($haystack, $needle, $offset);
/**
* Find length of initial segment matching mask
* @return int
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $str1 string
* @param $str2 string
* @param $start (optional) int
* @param $length (optional) int
*/
function strspn($str1, $str2, $start, $length);
/**
* Find first occurrence of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $haystack string
* @param $needle string
*/
function strstr($haystack, $needle);
/**
* Tokenize string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $token string
*/
function strtok($str, $token);
/**
* Make a string lowercase
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function strtolower($str);
/**
* Parse about any English textual datetime description into a Unix timestamp
* @return int
* @version PHP 3 >= 3.0.12, PHP 4, PHP 5
* @param $time string
* @param $now (optional) int
*/
function strtotime($time, $now);
/**
* Make a string uppercase
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
*/
function strtoupper($string);
/**
* Translate certain characters
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $from string
* @param $to string
*/
function strtr($str, $from, $to);
/**
* Get string value of a variable
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
*/
function strval($var);
/**
* Case-insensitive version of str_replace().
* @return mixed
* @version PHP 5
* @param $search mixed
* @param $replace mixed
* @param $subject mixed
* @param &$count (optional) int
*/
function str_ireplace($search, $replace, $subject, &$count);
/**
* Pad a string to a certain length with another string
* @return string
* @version PHP 4 >= 4.0.1, PHP 5
* @param $input string
* @param $pad_length int
* @param $pad_string (optional) string
* @param $pad_type (optional) int
*/
function str_pad($input, $pad_length, $pad_string, $pad_type);
/**
* Repeat a string
* @return string
* @version PHP 4, PHP 5
* @param $input string
* @param $multiplier int
*/
function str_repeat($input, $multiplier);
/**
* Replace all occurrences of the search string with the replacement string
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $search mixed
* @param $replace mixed
* @param $subject mixed
* @param &$count (optional) int
*/
function str_replace($search, $replace, $subject, &$count);
/**
* Perform the rot13 transform on a string
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $str string
*/
function str_rot13($str);
/**
* Randomly shuffles a string
* @return string
* @version PHP 4 >= 4.3.0, PHP 5
* @param $str string
*/
function str_shuffle($str);
/**
* Convert a string to an array
* @return array
* @version PHP 5
* @param $string string
* @param $split_length (optional) int
*/
function str_split($string, $split_length);
/**
* Return information about words used in a string
* @return mixed
* @version PHP 4 >= 4.3.0, PHP 5
* @param $string string
* @param $format (optional) int
* @param $charlist (optional) string
*/
function str_word_count($string, $format, $charlist);
/**
* Return part of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $string string
* @param $start int
* @param $length (optional) int
*/
function substr($string, $start, $length);
/**
* Binary safe optionally case insensitive comparison of 2 strings from an offset, up to length characters
* @return int
* @version PHP 5
* @param $main_str string
* @param $str string
* @param $offset int
* @param $length (optional) int
* @param $case_insensitivity (optional) bool
*/
function substr_compare($main_str, $str, $offset, $length, $case_insensitivity);
/**
* Count the number of substring occurrences
* @return int
* @version PHP 4, PHP 5
* @param $haystack string
* @param $needle string
* @param $offset (optional) int
* @param $length (optional) int
*/
function substr_count($haystack, $needle, $offset, $length);
/**
* Replace text within a portion of a string
* @return mixed
* @version PHP 4, PHP 5
* @param $string mixed
* @param $replacement string
* @param $start int
* @param $length (optional) int
*/
function substr_replace($string, $replacement, $start, $length);
/**
* Creates a new Action
* @return SWFAction
* @version PHP 4 >= 4.0.5
* @param $script string
*/
function SWFAction($script);
/**
* Loads Bitmap object
* @return SWFBitmap
* @version PHP 4 >= 4.0.5
* @param $file mixed
* @param $alphafile (optional) mixed
*/
function SWFBitmap($file, $alphafile);
/**
* Creates a new Button
* @return SWFButton
* @version PHP 4 >= 4.0.5
*/
function SWFbutton();
/**
* Loads SWFFill object
* @return SWFFill
* @version PHP 4 >= 4.0.5
*/
function SWFFill();
/**
* Loads a font definition
* @return SWFFont
* @version PHP 4 >= 4.0.5
* @param $filename string
*/
function SWFFont($filename);
/**
* Creates a gradient object
* @return SWFGradient
* @version PHP 4 >= 4.0.5
*/
function SWFGradient();
/**
* Creates a new SWFMorph object
* @return SWFMorph
* @version PHP 4 >= 4.0.5
*/
function SWFMorph();
/**
* Creates a new movie object, representing an SWF version 4 movie
* @return SWFMovie
* @version PHP 4 >= 4.0.5
*/
function SWFMovie();
/**
* Creates a new shape object
* @return SWFShape
* @version PHP 4 >= 4.0.5
*/
function SWFShape();
/**
* Creates a movie clip (a sprite)
* @return SWFSprite
* @version PHP 4 >= 4.0.5
*/
function SWFSprite();
/**
* Creates a new SWFText object
* @return SWFText
* @version PHP 4 >= 4.0.5
*/
function SWFText();
/**
* Creates a text field object
* @return SWFTextField
* @version PHP 4 >= 4.0.5
* @param $flags int
*/
function SWFTextField($flags);
/**
* Get a URL from a Shockwave Flash movie
* @return 
* @version PHP 4, PECL
* @param $url string
* @param $target string
*/
function swf_actiongeturl($url, $target);
/**
* Play a frame and then stop
* @return 
* @version PHP 4, PECL
* @param $framenumber int
*/
function swf_actiongotoframe($framenumber);
/**
* Display a frame with the specified label
* @return 
* @version PHP 4, PECL
* @param $label string
*/
function swf_actiongotolabel($label);
/**
* Go forward one frame
* @return 
* @version PHP 4, PECL
*/
function swf_actionnextframe();
/**
* Start playing the flash movie from the current frame
* @return 
* @version PHP 4, PECL
*/
function swf_actionplay();
/**
* Go backwards one frame
* @return 
* @version PHP 4, PECL
*/
function swf_actionprevframe();
/**
* Set the context for actions
* @return 
* @version PHP 4, PECL
* @param $target string
*/
function swf_actionsettarget($target);
/**
* Stop playing the flash movie at the current frame
* @return 
* @version PHP 4, PECL
*/
function swf_actionstop();
/**
* Toggle between low and high quality
* @return 
* @version PHP 4, PECL
*/
function swf_actiontogglequality();
/**
* Skip actions if a frame has not been loaded
* @return 
* @version PHP 4, PECL
* @param $framenumber int
* @param $skipcount int
*/
function swf_actionwaitforframe($framenumber, $skipcount);
/**
* Controls location, appearance and active area of the current button
* @return 
* @version PHP 4, PECL
* @param $states int
* @param $shapeid int
* @param $depth int
*/
function swf_addbuttonrecord($states, $shapeid, $depth);
/**
* Set the global add color to the rgba value specified
* @return 
* @version PHP 4, PECL
* @param $r float
* @param $g float
* @param $b float
* @param $a float
*/
function swf_addcolor($r, $g, $b, $a);
/**
* Close the current Shockwave Flash file
* @return 
* @version PHP 4, PECL
* @param $return_file int
*/
function swf_closefile($return_file);
/**
* Define a bitmap
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $image_name string
*/
function swf_definebitmap($objid, $image_name);
/**
* Defines a font
* @return 
* @version PHP 4, PECL
* @param $fontid int
* @param $fontname string
*/
function swf_definefont($fontid, $fontname);
/**
* Define a line
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $width float
*/
function swf_defineline($objid, $x1, $y1, $x2, $y2, $width);
/**
* Define a polygon
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $coords array
* @param $npoints int
* @param $width float
*/
function swf_definepoly($objid, $coords, $npoints, $width);
/**
* Define a rectangle
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $width float
*/
function swf_definerect($objid, $x1, $y1, $x2, $y2, $width);
/**
* Define a text string
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $str string
* @param $docenter int
*/
function swf_definetext($objid, $str, $docenter);
/**
* End the definition of the current button
* @return 
* @version PHP 4, PECL
*/
function swf_endbutton();
/**
* End the current action
* @return 
* @version PHP 4, PECL
*/
function swf_enddoaction();
/**
* Completes the definition of the current shape
* @return 
* @version PHP 4, PECL
*/
function swf_endshape();
/**
* End the definition of a symbol
* @return 
* @version PHP 4, PECL
*/
function swf_endsymbol();
/**
* Change the font size
* @return 
* @version PHP 4, PECL
* @param $size float
*/
function swf_fontsize($size);
/**
* Set the font slant
* @return 
* @version PHP 4, PECL
* @param $slant float
*/
function swf_fontslant($slant);
/**
* Set the current font tracking
* @return 
* @version PHP 4, PECL
* @param $tracking float
*/
function swf_fonttracking($tracking);
/**
* Get information about a bitmap
* @return array
* @version PHP 4, PECL
* @param $bitmapid int
*/
function swf_getbitmapinfo($bitmapid);
/**
* The height in pixels of a capital A and a lowercase x
* @return array
* @version PHP 4, PECL
*/
function swf_getfontinfo();
/**
* Get the frame number of the current frame
* @return int
* @version PHP 4, PECL
*/
function swf_getframe();
/**
* Label the current frame
* @return 
* @version PHP 4, PECL
* @param $name string
*/
function swf_labelframe($name);
/**
* Define a viewing transformation
* @return 
* @version PHP 4, PECL
* @param $view_x float
* @param $view_y float
* @param $view_z float
* @param $reference_x float
* @param $reference_y float
* @param $reference_z float
* @param $twist float
*/
function swf_lookat($view_x, $view_y, $view_z, $reference_x, $reference_y, $reference_z, $twist);
/**
* Modify an object
* @return 
* @version PHP 4, PECL
* @param $depth int
* @param $how int
*/
function swf_modifyobject($depth, $how);
/**
* Sets the global multiply color to the rgba value specified
* @return 
* @version PHP 4, PECL
* @param $r float
* @param $g float
* @param $b float
* @param $a float
*/
function swf_mulcolor($r, $g, $b, $a);
/**
* Returns the next free object id
* @return int
* @version PHP 4, PECL
*/
function swf_nextid();
/**
* Describe a transition used to trigger an action list
* @return 
* @version PHP 4, PECL
* @param $transition int
*/
function swf_oncondition($transition);
/**
* Open a new Shockwave Flash file
* @return 
* @version PHP 4, PECL
* @param $filename string
* @param $width float
* @param $height float
* @param $framerate float
* @param $r float
* @param $g float
* @param $b float
*/
function swf_openfile($filename, $width, $height, $framerate, $r, $g, $b);
/**
* Defines an orthographic mapping of user coordinates onto the current viewport
* @return 
* @version PHP 4 >= 4.0.1, PECL
* @param $xmin float
* @param $xmax float
* @param $ymin float
* @param $ymax float
* @param $zmin float
* @param $zmax float
*/
function swf_ortho($xmin, $xmax, $ymin, $ymax, $zmin, $zmax);
/**
* Defines 2D orthographic mapping of user coordinates onto the current viewport
* @return 
* @version PHP 4, PECL
* @param $xmin float
* @param $xmax float
* @param $ymin float
* @param $ymax float
*/
function swf_ortho2($xmin, $xmax, $ymin, $ymax);
/**
* Define a perspective projection transformation
* @return 
* @version PHP 4, PECL
* @param $fovy float
* @param $aspect float
* @param $near float
* @param $far float
*/
function swf_perspective($fovy, $aspect, $near, $far);
/**
* Place an object onto the screen
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $depth int
*/
function swf_placeobject($objid, $depth);
/**
* Define the viewer's position with polar coordinates
* @return 
* @version PHP 4, PECL
* @param $dist float
* @param $azimuth float
* @param $incidence float
* @param $twist float
*/
function swf_polarview($dist, $azimuth, $incidence, $twist);
/**
* Restore a previous transformation matrix
* @return 
* @version PHP 4, PECL
*/
function swf_popmatrix();
/**
* Enables or Disables the rounding of the translation when objects are placed or moved
* @return 
* @version PHP 4, PECL
* @param $round int
*/
function swf_posround($round);
/**
* Push the current transformation matrix back unto the stack
* @return 
* @version PHP 4, PECL
*/
function swf_pushmatrix();
/**
* Remove an object
* @return 
* @version PHP 4, PECL
* @param $depth int
*/
function swf_removeobject($depth);
/**
* Rotate the current transformation
* @return 
* @version PHP 4, PECL
* @param $angle float
* @param $axis string
*/
function swf_rotate($angle, $axis);
/**
* Scale the current transformation
* @return 
* @version PHP 4, PECL
* @param $x float
* @param $y float
* @param $z float
*/
function swf_scale($x, $y, $z);
/**
* Change the current font
* @return 
* @version PHP 4, PECL
* @param $fontid int
*/
function swf_setfont($fontid);
/**
* Switch to a specified frame
* @return 
* @version PHP 4, PECL
* @param $framenumber int
*/
function swf_setframe($framenumber);
/**
* Draw a circular arc
* @return 
* @version PHP 4, PECL
* @param $x float
* @param $y float
* @param $r float
* @param $ang1 float
* @param $ang2 float
*/
function swf_shapearc($x, $y, $r, $ang1, $ang2);
/**
* Draw a quadratic bezier curve between two points
* @return 
* @version PHP 4, PECL
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
*/
function swf_shapecurveto($x1, $y1, $x2, $y2);
/**
* Draw a cubic bezier curve
* @return 
* @version PHP 4, PECL
* @param $x1 float
* @param $y1 float
* @param $x2 float
* @param $y2 float
* @param $x3 float
* @param $y3 float
*/
function swf_shapecurveto3($x1, $y1, $x2, $y2, $x3, $y3);
/**
* Set current fill mode to clipped bitmap
* @return 
* @version PHP 4, PECL
* @param $bitmapid int
*/
function swf_shapefillbitmapclip($bitmapid);
/**
* Set current fill mode to tiled bitmap
* @return 
* @version PHP 4, PECL
* @param $bitmapid int
*/
function swf_shapefillbitmaptile($bitmapid);
/**
* Turns off filling
* @return 
* @version PHP 4, PECL
*/
function swf_shapefilloff();
/**
* Set the current fill style to the specified color
* @return 
* @version PHP 4, PECL
* @param $r float
* @param $g float
* @param $b float
* @param $a float
*/
function swf_shapefillsolid($r, $g, $b, $a);
/**
* Set the current line style
* @return 
* @version PHP 4, PECL
* @param $r float
* @param $g float
* @param $b float
* @param $a float
* @param $width float
*/
function swf_shapelinesolid($r, $g, $b, $a, $width);
/**
* Draw a line
* @return 
* @version PHP 4, PECL
* @param $x float
* @param $y float
*/
function swf_shapelineto($x, $y);
/**
* Move the current position
* @return 
* @version PHP 4, PECL
* @param $x float
* @param $y float
*/
function swf_shapemoveto($x, $y);
/**
* Display the current frame
* @return 
* @version PHP 4, PECL
*/
function swf_showframe();
/**
* Start the definition of a button
* @return 
* @version PHP 4, PECL
* @param $objid int
* @param $type int
*/
function swf_startbutton($objid, $type);
/**
* Start a description of an action list for the current frame
* @return 
* @version PHP 4, PECL
*/
function swf_startdoaction();
/**
* Start a complex shape
* @return 
* @version PHP 4, PECL
* @param $objid int
*/
function swf_startshape($objid);
/**
* Define a symbol
* @return 
* @version PHP 4, PECL
* @param $objid int
*/
function swf_startsymbol($objid);
/**
* Get the width of a string
* @return float
* @version PHP 4, PECL
* @param $str string
*/
function swf_textwidth($str);
/**
* Translate the current transformations
* @return 
* @version PHP 4, PECL
* @param $x float
* @param $y float
* @param $z float
*/
function swf_translate($x, $y, $z);
/**
* Select an area for future drawing
* @return 
* @version PHP 4, PECL
* @param $xmin float
* @param $xmax float
* @param $ymin float
* @param $ymax float
*/
function swf_viewport($xmin, $xmax, $ymin, $ymax);
/**
* Gets number of affected rows in last query
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $link_identifier resource
*/
function sybase_affected_rows($link_identifier);
/**
* Closes a Sybase connection
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $link_identifier resource
*/
function sybase_close($link_identifier);
/**
* Opens a Sybase server connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $servername string
* @param $username (optional) string
* @param $password (optional) string
* @param $charset (optional) string
* @param $appname (optional) string
*/
function sybase_connect($servername, $username, $password, $charset, $appname);
/**
* Moves internal row pointer
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result_identifier resource
* @param $row_number int
*/
function sybase_data_seek($result_identifier, $row_number);
/**
* Sets the deadlock retry count
* @return 
* @version PHP 4 >= 4.3.0, PHP 5
* @param $retry_count int
*/
function sybase_deadlock_retry_count($retry_count);
/**
* Fetch row as array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function sybase_fetch_array($result);
/**
* Fetch a result row as an associative array
* @return array
* @version PHP 4 >= 4.3.0, PHP 5
* @param $result resource
*/
function sybase_fetch_assoc($result);
/**
* Get field information from a result
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset (optional) int
*/
function sybase_fetch_field($result, $field_offset);
/**
* Fetch a row as an object
* @return object
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $object (optional) mixed
*/
function sybase_fetch_object($result, $object);
/**
* Get a result row as an enumerated array
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function sybase_fetch_row($result);
/**
* Sets field offset
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $field_offset int
*/
function sybase_field_seek($result, $field_offset);
/**
* Frees result memory
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function sybase_free_result($result);
/**
* Returns the last message from the server
* @return string
* @version PHP 3, PHP 4, PHP 5
*/
function sybase_get_last_message();
/**
* Sets minimum client severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function sybase_min_client_severity($severity);
/**
* Sets minimum error severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function sybase_min_error_severity($severity);
/**
* Sets minimum message severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function sybase_min_message_severity($severity);
/**
* Sets minimum server severity
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $severity int
*/
function sybase_min_server_severity($severity);
/**
* Gets the number of fields in a result set
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function sybase_num_fields($result);
/**
* Get number of rows in a result set
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
*/
function sybase_num_rows($result);
/**
* Open persistent Sybase connection
* @return resource
* @version PHP 3, PHP 4, PHP 5
* @param $servername string
* @param $username (optional) string
* @param $password (optional) string
* @param $charset (optional) string
* @param $appname (optional) string
*/
function sybase_pconnect($servername, $username, $password, $charset, $appname);
/**
* Sends a Sybase query
* @return mixed
* @version PHP 3, PHP 4, PHP 5
* @param $query string
* @param $link_identifier (optional) resource
*/
function sybase_query($query, $link_identifier);
/**
* Get result data
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $result resource
* @param $row int
* @param $field mixed
*/
function sybase_result($result, $row, $field);
/**
* Selects a Sybase database
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $database_name string
* @param $link_identifier (optional) resource
*/
function sybase_select_db($database_name, $link_identifier);
/**
* Sets the handler called when a server message is raised
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $handler callback
* @param $connection (optional) resource
*/
function sybase_set_message_handler($handler, $connection);
/**
* Send a Sybase query and do not block
* @return resource
* @version PHP 4 >= 4.3.0, PHP 5
* @param $query string
* @param $link_identifier resource
* @param $store_result (optional) bool
*/
function sybase_unbuffered_query($query, $link_identifier, $store_result);
/**
* Creates a symbolic link
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $target string
* @param $link string
*/
function symlink($target, $link);
/**
* Generate a system log message
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $priority int
* @param $message string
*/
function syslog($priority, $message);
/**
* Execute an external program and display the output
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $command string
* @param &$return_var (optional) int
*/
function system($command, &$return_var);
/**
* Tangent
* @return float
* @version PHP 3, PHP 4, PHP 5
* @param $arg float
*/
function tan($arg);
/**
* Hyperbolic tangent
* @return float
* @version PHP 4 >= 4.1.0, PHP 5
* @param $arg float
*/
function tanh($arg);
/**
* Performs a tcpwrap check
* @return bool
* @version PECL
* @param $daemon string
* @param $address string
* @param $user (optional) string
* @param $nodns (optional) bool
*/
function tcpwrap_check($daemon, $address, $user, $nodns);
/**
* Create file with unique file name
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $dir string
* @param $prefix string
*/
function tempnam($dir, $prefix);
/**
* Sets the default domain
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $text_domain string
*/
function textdomain($text_domain);
/**
* Returns the Number of Tidy accessibility warnings encountered for specified document
* @return int
* @version PHP 5
* @param $object tidy
*/
function tidy_access_count($object);
/**
* Execute configured cleanup and repair operations on parsed markup
* @return Procedural
* @version PHP 5
*/
function tidy_clean_repair();
/**
* Returns the Number of Tidy configuration errors encountered for specified document
* @return int
* @version PHP 5
* @param $object tidy
*/
function tidy_config_count($object);
/**
* Run configured diagnostics on parsed and repaired markup
* @return Procedural
* @version PHP 5
*/
function tidy_diagnose();
/**
* Returns the Number of Tidy errors encountered for specified document
* @return int
* @version PHP 5
* @param $object tidy
*/
function tidy_error_count($object);
/**
* Returns the value of the specified configuration option for the tidy document
* @return Procedural
* @version PHP 5
*/
function tidy_getopt();
/**
* Returns a tidyNode Object starting from the <body> tag of the tidy parse tree
* @return Procedural
* @version PHP 5
*/
function tidy_get_body();
/**
* Get current Tidy configuration
* @return Procedural
* @version PHP 5
*/
function tidy_get_config();
/**
* Return warnings and errors which occurred parsing the specified document
* @return Procedural
* @version PHP 5
*/
function tidy_get_error_buffer();
/**
* Returns a tidyNode Object starting from the <head> tag of the tidy parse tree
* @return Procedural
* @version PHP 5
*/
function tidy_get_head();
/**
* Returns a tidyNode Object starting from the <html> tag of the tidy parse tree
* @return Procedural
* @version PHP 5
*/
function tidy_get_html();
/**
* Get the Detected HTML version for the specified document
* @return Procedural
* @version PHP 5
*/
function tidy_get_html_ver();
/**
* Returns the documentation for the given option name
* @return Procedural
* @version PHP 5 >= 5.1.0RC1
*/
function tidy_get_opt_doc();
/**
* Return a string representing the parsed tidy markup
* @return string
* @version PHP 5
* @param $object tidy
*/
function tidy_get_output($object);
/**
* Get release date (version) for Tidy library
* @return Procedural
* @version PHP 5
*/
function tidy_get_release();
/**
* Returns a tidyNode object representing the root of the tidy parse tree
* @return Procedural
* @version PHP 5
*/
function tidy_get_root();
/**
* Get status of specified document
* @return Procedural
* @version PHP 5
*/
function tidy_get_status();
/**
* Indicates if the document is a XHTML document
* @return Procedural
* @version PHP 5
*/
function tidy_is_xhtml();
/**
* Indicates if the document is a generic (non HTML/XHTML) XML document
* @return Procedural
* @version PHP 5
*/
function tidy_is_xml();
/**
* Parse markup in file or URI
* @return Procedural
* @version PHP 5
*/
function tidy_parse_file();
/**
* Parse a document stored in a string
* @return Procedural
* @version PHP 5
*/
function tidy_parse_string();
/**
* Repair a file and return it as a string
* @return string
* @version PHP 5
* @param $filename string
* @param $config (optional) mixed
* @param $encoding (optional) string
* @param $use_include_path (optional) bool
*/
function tidy_repair_file($filename, $config, $encoding, $use_include_path);
/**
* Repair a string using an optionally provided configuration file
* @return string
* @version PHP 5
* @param $data string
* @param $config (optional) mixed
* @param $encoding (optional) string
*/
function tidy_repair_string($data, $config, $encoding);
/**
* Returns the Number of Tidy warnings encountered for specified document
* @return int
* @version PHP 5
* @param $object tidy
*/
function tidy_warning_count($object);
/**
* Return current Unix timestamp
* @return int
* @version PHP 3, PHP 4, PHP 5
*/
function time();
/**
* Delay for a number of seconds and nanoseconds
* @return mixed
* @version PHP 5
* @param $seconds int
* @param $nanoseconds int
*/
function time_nanosleep($seconds, $nanoseconds);
/**
* Make the script sleep until the specified time
* @return bool
* @version PHP 5 >= 5.1.0RC1
* @param $timestamp float
*/
function time_sleep_until($timestamp);
/**
* Creates a temporary file
* @return resource
* @version PHP 3 >= 3.0.13, PHP 4, PHP 5
*/
function tmpfile();
/**
* Split given source into PHP tokens
* @return array
* @version PHP 4 >= 4.2.0, PHP 5
* @param $source string
*/
function token_get_all($source);
/**
* Get the symbolic name of a given PHP token
* @return string
* @version PHP 4 >= 4.2.0, PHP 5
* @param $token int
*/
function token_name($token);
/**
* Sets access and modification time of file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $time (optional) int
* @param $atime (optional) int
*/
function touch($filename, $time, $atime);
/**
* Generates a user-level error/warning/notice message
* @return bool
* @version PHP 4 >= 4.0.1, PHP 5
* @param $error_msg string
* @param $error_type (optional) int
*/
function trigger_error($error_msg, $error_type);
/**
* Strip whitespace (or other characters) from the beginning and end of a string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
* @param $charlist (optional) string
*/
function trim($str, $charlist);
/**
* Sort an array with a user-defined comparison function and maintain index association
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param &$array array
* @param $cmp_function callback
*/
function uasort(&$array, $cmp_function);
/**
* Make a string's first character uppercase
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function ucfirst($str);
/**
* Uppercase the first character of each word in a string
* @return string
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param $str string
*/
function ucwords($str);
/**
* Add various search limits
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
* @param $var int
* @param $val string
*/
function udm_add_search_limit($agent, $var, $val);
/**
* Allocate mnoGoSearch session
* @return resource
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $dbaddr string
* @param $dbmode (optional) string
*/
function udm_alloc_agent($dbaddr, $dbmode);
/**
* Allocate mnoGoSearch session
* @return resource
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $databases array
*/
function udm_alloc_agent_array($databases);
/**
* Get mnoGoSearch API version
* @return int
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
*/
function udm_api_version();
/**
* Get all the categories on the same level with the current one
* @return array
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $agent resource
* @param $category string
*/
function udm_cat_list($agent, $category);
/**
* Get the path to the current category
* @return array
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $agent resource
* @param $category string
*/
function udm_cat_path($agent, $category);
/**
* Check if the given charset is known to mnogosearch
* @return bool
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $agent resource
* @param $charset string
*/
function udm_check_charset($agent, $charset);
/**
* Check connection to stored
* @return int
* @version PHP 4 >= 4.2.0
* @param $agent resource
* @param $link int
* @param $doc_id string
*/
function udm_check_stored($agent, $link, $doc_id);
/**
* Clear all mnoGoSearch search restrictions
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
*/
function udm_clear_search_limits($agent);
/**
* Close connection to stored
* @return int
* @version PHP 4 >= 4.2.0
* @param $agent resource
* @param $link int
*/
function udm_close_stored($agent, $link);
/**
* Return CRC32 checksum of given string
* @return int
* @version PHP 4 >= 4.2.0, PHP 5 <= 5.0.4
* @param $agent resource
* @param $str string
*/
function udm_crc32($agent, $str);
/**
* Get mnoGoSearch error number
* @return int
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
*/
function udm_errno($agent);
/**
* Get mnoGoSearch error message
* @return string
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
*/
function udm_error($agent);
/**
* Perform search
* @return resource
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
* @param $query string
*/
function udm_find($agent, $query);
/**
* Free mnoGoSearch session
* @return int
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
*/
function udm_free_agent($agent);
/**
* Free memory allocated for ispell data
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent int
*/
function udm_free_ispell_data($agent);
/**
* Free mnoGoSearch result
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $res resource
*/
function udm_free_res($res);
/**
* Get total number of documents in database
* @return int
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
*/
function udm_get_doc_count($agent);
/**
* Fetch mnoGoSearch result field
* @return string
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $res resource
* @param $row int
* @param $field int
*/
function udm_get_res_field($res, $row, $field);
/**
* Get mnoGoSearch result parameters
* @return string
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $res resource
* @param $param int
*/
function udm_get_res_param($res, $param);
/**
* Return Hash32 checksum of gived string
* @return int
* @version PHP 4 >= 4.3.3, PHP 5 <= 5.0.4
* @param $agent resource
* @param $str string
*/
function udm_hash32($agent, $str);
/**
* Load ispell data
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
* @param $var int
* @param $val1 string
* @param $val2 string
* @param $flag int
*/
function udm_load_ispell_data($agent, $var, $val1, $val2, $flag);
/**
* Open connection to stored
* @return int
* @version PHP 4 >= 4.2.0
* @param $agent resource
* @param $storedaddr string
*/
function udm_open_stored($agent, $storedaddr);
/**
* Set mnoGoSearch agent session parameters
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5 <= 5.0.4
* @param $agent resource
* @param $var int
* @param $val string
*/
function udm_set_agent_param($agent, $var, $val);
/**
* Sort an array by keys using a user-defined comparison function
* @return bool
* @version PHP 3 >= 3.0.4, PHP 4, PHP 5
* @param &$array array
* @param $cmp_function callback
*/
function uksort(&$array, $cmp_function);
/**
* Changes the current umask
* @return int
* @version PHP 3, PHP 4, PHP 5
* @param $mask int
*/
function umask($mask);
/**
* Takes a unicode string and converts it to a string in the specified encoding
* @return string
* @version PHP 5 CVS only
* @param $input unicode
* @param $encoding string
*/
function unicode_encode($input, $encoding);
/**
* Generate a unique ID
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $prefix string
* @param $more_entropy (optional) bool
*/
function uniqid($prefix, $more_entropy);
/**
* Convert Unix timestamp to Julian Day
* @return int
* @version PHP 4, PHP 5
* @param $timestamp int
*/
function unixtojd($timestamp);
/**
* Deletes a file
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
* @param $context (optional) resource
*/
function unlink($filename, $context);
/**
* Unpack data from binary string
* @return array
* @version PHP 3, PHP 4, PHP 5
* @param $format string
* @param $data string
*/
function unpack($format, $data);
/**
* De-register a function for execution on each tick
* @return 
* @version PHP 4 >= 4.0.3, PHP 5
* @param $function_name string
*/
function unregister_tick_function($function_name);
/**
* Creates a PHP value from a stored representation
* @return mixed
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
* @param $str string
*/
function unserialize($str);
/**
* Unset a given variable
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $var mixed
* @param $var (optional) mixed
* @param $params1 (optional) mixed
*/
function unset($var, $var, $params1);
/**
* Decodes URL-encoded string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function urldecode($str);
/**
* URL-encodes string
* @return string
* @version PHP 3, PHP 4, PHP 5
* @param $str string
*/
function urlencode($str);
/**
* Alias of trigger_error()
* @return &#13;
* @version PHP 4, PHP 5
*/
function user_error();
/**
* Set whether to use the SOAP error handler and return the former value
* @return bool
* @version PHP 5
* @param $handler bool
*/
function use_soap_error_handler($handler);
/**
* Delay execution in microseconds
* @return 
* @version PHP 3, PHP 4, PHP 5
* @param $micro_seconds int
*/
function usleep($micro_seconds);
/**
* Sort an array by values using a user-defined comparison function
* @return bool
* @version PHP 3 >= 3.0.3, PHP 4, PHP 5
* @param &$array array
* @param $cmp_function callback
*/
function usort(&$array, $cmp_function);
/**
* Converts a string with ISO-8859-1 characters encoded with UTF-8 to single-byte ISO-8859-1
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $data string
*/
function utf8_decode($data);
/**
* Encodes an ISO-8859-1 string to UTF-8
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $data string
*/
function utf8_encode($data);
/**
* Returns the absolute value of a variant
* @return mixed
* @version PHP 5
* @param $val mixed
*/
function variant_abs($val);
/**
* "Adds" two variant values together and returns the result
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_add($left, $right);
/**
* performs a bitwise AND operation between two variants and returns the result
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_and($left, $right);
/**
* Convert a variant into a new variant object of another type
* @return variant
* @version PHP 5
* @param $variant variant
* @param $type int
*/
function variant_cast($variant, $type);
/**
* concatenates two variant values together and returns the result
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_cat($left, $right);
/**
* Compares two variants
* @return int
* @version PHP 5
* @param $left mixed
* @param $right mixed
* @param $lcid (optional) int
* @param $flags (optional) int
*/
function variant_cmp($left, $right, $lcid, $flags);
/**
* Returns a variant date representation of a unix timestamp
* @return variant
* @version PHP 5
* @param $timestamp int
*/
function variant_date_from_timestamp($timestamp);
/**
* Converts a variant date/time value to unix timestamp
* @return int
* @version PHP 5
* @param $variant variant
*/
function variant_date_to_timestamp($variant);
/**
* Returns the result from dividing two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_div($left, $right);
/**
* Performs a bitwise equivalence on two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_eqv($left, $right);
/**
* Returns the integer portion ? of a variant
* @return mixed
* @version PHP 5
* @param $variant mixed
*/
function variant_fix($variant);
/**
* Returns the type of a variant object
* @return int
* @version PHP 5
* @param $variant variant
*/
function variant_get_type($variant);
/**
* Converts variants to integers and then returns the result from dividing them
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_idiv($left, $right);
/**
* Performs a bitwise implication on two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_imp($left, $right);
/**
* Returns the integer portion of a variant
* @return mixed
* @version PHP 5
* @param $variant mixed
*/
function variant_int($variant);
/**
* Divides two variants and returns only the remainder
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_mod($left, $right);
/**
* multiplies the values of the two variants and returns the result
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_mul($left, $right);
/**
* Performs logical negation on a variant
* @return mixed
* @version PHP 5
* @param $variant mixed
*/
function variant_neg($variant);
/**
* Performs bitwise not negation on a variant
* @return mixed
* @version PHP 5
* @param $variant mixed
*/
function variant_not($variant);
/**
* Performs a logical disjunction on two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_or($left, $right);
/**
* Returns the result of performing the power function with two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_pow($left, $right);
/**
* Rounds a variant to the specified number of decimal places
* @return mixed
* @version PHP 5
* @param $variant mixed
* @param $decimals int
*/
function variant_round($variant, $decimals);
/**
* Assigns a new value for a variant object
* @return 
* @version PHP 5
* @param $variant variant
* @param $value mixed
*/
function variant_set($variant, $value);
/**
* Convert a variant into another type "in-place"
* @return 
* @version PHP 5
* @param $variant variant
* @param $type int
*/
function variant_set_type($variant, $type);
/**
* subtracts the value of the right variant from the left variant value and returns the result
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_sub($left, $right);
/**
* Performs a logical exclusion on two variants
* @return mixed
* @version PHP 5
* @param $left mixed
* @param $right mixed
*/
function variant_xor($left, $right);
/**
* Dumps information about a variable
* @return 
* @version PHP 3 >= 3.0.5, PHP 4, PHP 5
* @param $expression mixed
* @param $expression (optional) mixed
* @param $params1 (optional) 
*/
function var_dump($expression, $expression, $params1);
/**
* Outputs or returns a parsable string representation of a variable
* @return mixed
* @version PHP 4 >= 4.2.0, PHP 5
* @param $expression mixed
* @param $return (optional) bool
*/
function var_export($expression, $return);
/**
* Compares two "PHP-standardized" version number strings
* @return mixed
* @version PHP 4 >= 4.1.0, PHP 5
* @param $version1 string
* @param $version2 string
* @param $operator (optional) string
*/
function version_compare($version1, $version2, $operator);
/**
* Write a formatted string to a stream
* @return int
* @version PHP 5
* @param $handle resource
* @param $format string
* @param $args array
*/
function vfprintf($handle, $format, $args);
/**
* Perform an Apache sub-request
* @return bool
* @version PHP 3, PHP 4, PHP 5
* @param $filename string
*/
function virtual($filename);
/**
* Add an alias for a virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $domain string
* @param $aliasdomain string
*/
function vpopmail_add_alias_domain($domain, $aliasdomain);
/**
* Add alias to an existing virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $olddomain string
* @param $newdomain string
*/
function vpopmail_add_alias_domain_ex($olddomain, $newdomain);
/**
* Add a new virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $domain string
* @param $dir string
* @param $uid int
* @param $gid int
*/
function vpopmail_add_domain($domain, $dir, $uid, $gid);
/**
* Add a new virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $domain string
* @param $passwd string
* @param $quota (optional) string
* @param $bounce (optional) string
* @param $apop (optional) bool
*/
function vpopmail_add_domain_ex($domain, $passwd, $quota, $bounce, $apop);
/**
* Add a new user to the specified virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
* @param $password string
* @param $gecos (optional) string
* @param $apop (optional) bool
*/
function vpopmail_add_user($user, $domain, $password, $gecos, $apop);
/**
* Insert a virtual alias
* @return bool
* @version 4.1.0 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
* @param $alias string
*/
function vpopmail_alias_add($user, $domain, $alias);
/**
* Deletes all virtual aliases of a user
* @return bool
* @version 4.1.0 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
*/
function vpopmail_alias_del($user, $domain);
/**
* Deletes all virtual aliases of a domain
* @return bool
* @version 4.1.0 - 4.2.3 only, PECL
* @param $domain string
*/
function vpopmail_alias_del_domain($domain);
/**
* Get all lines of an alias for a domain
* @return array
* @version 4.1.0 - 4.2.3 only, PECL
* @param $alias string
* @param $domain string
*/
function vpopmail_alias_get($alias, $domain);
/**
* Get all lines of an alias for a domain
* @return array
* @version 4.1.0 - 4.2.3 only, PECL
* @param $domain string
*/
function vpopmail_alias_get_all($domain);
/**
* Attempt to validate a username/domain/password
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
* @param $password string
* @param $apop (optional) string
*/
function vpopmail_auth_user($user, $domain, $password, $apop);
/**
* Delete a virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $domain string
*/
function vpopmail_del_domain($domain);
/**
* Delete a virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $domain string
*/
function vpopmail_del_domain_ex($domain);
/**
* Delete a user from a virtual domain
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
*/
function vpopmail_del_user($user, $domain);
/**
* Get text message for last vpopmail error
* @return string
* @version 4.0.5 - 4.2.3 only, PECL
*/
function vpopmail_error();
/**
* Change a virtual user's password
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
* @param $password string
* @param $apop (optional) bool
*/
function vpopmail_passwd($user, $domain, $password, $apop);
/**
* Sets a virtual user's quota
* @return bool
* @version 4.0.5 - 4.2.3 only, PECL
* @param $user string
* @param $domain string
* @param $quota string
*/
function vpopmail_set_user_quota($user, $domain, $quota);
/**
* Output a formatted string
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $format string
* @param $args array
*/
function vprintf($format, $args);
/**
* Return a formatted string
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $format string
* @param $args array
*/
function vsprintf($format, $args);
/**
* Defines a type for use with other w32api_functions
* @return bool
* @version 4.2.0 - 4.2.3 only
* @param $typename string
* @param $member1_type string
* @param $member1_name string
* @param $params1 (optional) string
* @param $params2 (optional) string
*/
function w32api_deftype($typename, $member1_type, $member1_name, $params1, $params2);
/**
* Creates an instance of the data type typename and fills it with the values passed
* @return resource
* @version 4.2.0 - 4.2.3 only
* @param $typename string
* @param $value mixed
* @param $params1 (optional) mixed
*/
function w32api_init_dtype($typename, $value, $params1);
/**
* Invokes function funcname with the arguments passed after the function name
* @return mixed
* @version 4.2.0 - 4.2.3 only
* @param $funcname string
* @param $argument mixed
* @param $params1 (optional) mixed
*/
function w32api_invoke_function($funcname, $argument, $params1);
/**
* Registers function function_name from library with PHP
* @return bool
* @version 4.2.0 - 4.2.3 only
* @param $library string
* @param $function_name string
* @param $return_type string
*/
function w32api_register_function($library, $function_name, $return_type);
/**
* Sets the calling method used
* @return 
* @version 4.2.0 - 4.2.3 only
* @param $method int
*/
function w32api_set_call_method($method);
/**
* Add variables to a WDDX packet with the specified ID
* @return bool
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $packet_id int
* @param $name_var mixed
* @param $params1 (optional) mixed
*/
function wddx_add_vars($packet_id, $name_var, $params1);
/**
* Alias of wddx_unserialize()
* @return &#13;
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
*/
function wddx_deserialize();
/**
* Ends a WDDX packet with the specified ID
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $packet_id resource
*/
function wddx_packet_end($packet_id);
/**
* Starts a new WDDX packet with structure inside it
* @return resource
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $comment string
*/
function wddx_packet_start($comment);
/**
* Serialize a single value into a WDDX packet
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $var mixed
* @param $comment (optional) string
*/
function wddx_serialize_value($var, $comment);
/**
* Serialize variables into a WDDX packet
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5
* @param $var_name mixed
* @param $params1 (optional) mixed
*/
function wddx_serialize_vars($var_name, $params1);
/**
* Unserializes a WDDX packet
* @return mixed
* @version PHP 3 >= 3.0.7, PHP 5 CVS only
* @param $packet string
*/
function wddx_unserialize($packet);
/**
* Creates a new service entry in the SCM database
* @return int
* @version PECL
* @param $details array
* @param $machine (optional) string
*/
function win32_create_service($details, $machine);
/**
* Deletes a service entry from the SCM database
* @return int
* @version PECL
* @param $servicename string
* @param $machine (optional) string
*/
function win32_delete_service($servicename, $machine);
/**
* Returns the last control message that was sent to this service
* @return int
* @version PECL
*/
function win32_get_last_control_message();
/**
* Queries the status of a service
* @return mixed
* @version PECL
* @param $servicename string
* @param $machine (optional) string
*/
function win32_query_service_status($servicename, $machine);
/**
* Update the service status
* @return bool
* @version PECL
* @param $status int
*/
function win32_set_service_status($status);
/**
* Starts a service
* @return int
* @version PECL
* @param $servicename string
* @param $machine (optional) string
*/
function win32_start_service($servicename, $machine);
/**
* Registers the script with the SCM, so that it can act as the service with the given name
* @return bool
* @version PECL
* @param $name string
*/
function win32_start_service_ctrl_dispatcher($name);
/**
* Stops a service
* @return int
* @version PECL
* @param $servicename string
* @param $machine (optional) string
*/
function win32_stop_service($servicename, $machine);
/**
* Wraps a string to a given number of characters using a string break character
* @return string
* @version PHP 4 >= 4.0.2, PHP 5
* @param $str string
* @param $width (optional) int
* @param $break (optional) string
* @param $cut (optional) bool
*/
function wordwrap($str, $width, $break, $cut);
/**
* Get an extended attribute
* @return string
* @version PECL
* @param $filename string
* @param $name string
* @param $flags (optional) int
*/
function xattr_get($filename, $name, $flags);
/**
* Get a list of extended attributes
* @return array
* @version PECL
* @param $filename string
* @param $flags (optional) int
*/
function xattr_list($filename, $flags);
/**
* Remove an extended attribute
* @return bool
* @version PECL
* @param $filename string
* @param $name string
* @param $flags (optional) int
*/
function xattr_remove($filename, $name, $flags);
/**
* Set an extended attribute
* @return bool
* @version PECL
* @param $filename string
* @param $name string
* @param $value string
* @param $flags (optional) int
*/
function xattr_set($filename, $name, $value, $flags);
/**
* Check if filesystem supports extended attributes
* @return bool
* @version PECL
* @param $filename string
* @param $flags (optional) int
*/
function xattr_supported($filename, $flags);
/**
* Make unified diff of two files
* @return bool
* @version PECL
* @param $file1 string
* @param $file2 string
* @param $dest string
* @param $context (optional) int
* @param $minimal (optional) bool
*/
function xdiff_file_diff($file1, $file2, $dest, $context, $minimal);
/**
* Make binary diff of two files
* @return bool
* @version PECL
* @param $file1 string
* @param $file2 string
* @param $dest string
*/
function xdiff_file_diff_binary($file1, $file2, $dest);
/**
* Merge 3 files into one
* @return mixed
* @version PECL
* @param $file1 string
* @param $file2 string
* @param $file3 string
* @param $dest string
*/
function xdiff_file_merge3($file1, $file2, $file3, $dest);
/**
* Patch a file with an unified diff
* @return mixed
* @version PECL
* @param $file string
* @param $patch string
* @param $dest string
* @param $flags (optional) int
*/
function xdiff_file_patch($file, $patch, $dest, $flags);
/**
* Patch a file with a binary diff
* @return bool
* @version PECL
* @param $file string
* @param $patch string
* @param $dest string
*/
function xdiff_file_patch_binary($file, $patch, $dest);
/**
* Make unified diff of two strings
* @return string
* @version PECL
* @param $str1 string
* @param $str2 string
* @param $context (optional) int
* @param $minimal (optional) bool
*/
function xdiff_string_diff($str1, $str2, $context, $minimal);
/**
* Make binary diff of two strings
* @return string
* @version PECL
* @param $str1 string
* @param $str2 string
*/
function xdiff_string_diff_binary($str1, $str2);
/**
* Merge 3 strings into one
* @return mixed
* @version PECL
* @param $str1 string
* @param $str2 string
* @param $str3 string
* @param &$error (optional) string
*/
function xdiff_string_merge3($str1, $str2, $str3, &$error);
/**
* Patch a string with an unified diff
* @return string
* @version PECL
* @param $str string
* @param $patch string
* @param $flags (optional) int
* @param &$error (optional) string
*/
function xdiff_string_patch($str, $patch, $flags, &$error);
/**
* Patch a string with a binary diff
* @return string
* @version PECL
* @param $str string
* @param $patch string
*/
function xdiff_string_patch_binary($str, $patch);
/**
* Decodes XML into native PHP types
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $xml string
* @param $encoding (optional) string
*/
function xmlrpc_decode($xml, $encoding);
/**
* Decodes XML into native PHP types
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $xml string
* @param &$method string
* @param $encoding (optional) string
*/
function xmlrpc_decode_request($xml, &$method, $encoding);
/**
* Generates XML for a PHP value
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $value mixed
*/
function xmlrpc_encode($value);
/**
* Generates XML for a method request
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $method string
* @param $params mixed
* @param $output_options (optional) array
*/
function xmlrpc_encode_request($method, $params, $output_options);
/**
* Gets xmlrpc type for a PHP value
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $value mixed
*/
function xmlrpc_get_type($value);
/**
* Determines if an array value represents an XMLRPC fault
* @return bool
* @version PHP 4 >= 4.3.0, PHP 5
* @param $arg array
*/
function xmlrpc_is_fault($arg);
/**
* Decodes XML into a list of method descriptions
* @return array
* @version PHP 4 >= 4.1.0, PHP 5
* @param $xml string
*/
function xmlrpc_parse_method_descriptions($xml);
/**
* Adds introspection documentation
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $server resource
* @param $desc array
*/
function xmlrpc_server_add_introspection_data($server, $desc);
/**
* Parses XML requests and call methods
* @return string
* @version PHP 4 >= 4.1.0, PHP 5
* @param $server resource
* @param $xml string
* @param $user_data mixed
* @param $output_options (optional) array
*/
function xmlrpc_server_call_method($server, $xml, $user_data, $output_options);
/**
* Creates an xmlrpc server
* @return resource
* @version PHP 4 >= 4.1.0, PHP 5
*/
function xmlrpc_server_create();
/**
* Destroys server resources
* @return int
* @version PHP 4 >= 4.1.0, PHP 5
* @param $server resource
*/
function xmlrpc_server_destroy($server);
/**
* Register a PHP function to generate documentation
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $server resource
* @param $function string
*/
function xmlrpc_server_register_introspection_callback($server, $function);
/**
* Register a PHP function to resource method matching method_name
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param $server resource
* @param $method_name string
* @param $function string
*/
function xmlrpc_server_register_method($server, $method_name, $function);
/**
* Sets xmlrpc type, base64 or datetime, for a PHP string value
* @return bool
* @version PHP 4 >= 4.1.0, PHP 5
* @param &$value string
* @param $type string
*/
function xmlrpc_set_type(&$value, $type);
/**
* End attribute - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_attribute($xmlwriter);
/**
* End current CDATA - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_cdata($xmlwriter);
/**
* Create end comment - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_comment($xmlwriter);
/**
* End current document - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_document($xmlwriter);
/**
* End current DTD - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_dtd($xmlwriter);
/**
* End current DTD element - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_dtd_element($xmlwriter);
/**
* End current element - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_element($xmlwriter);
/**
* End current PI - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_end_pi($xmlwriter);
/**
* Output current buffer
* @return mixed
* @version PECL
* @param $xmlwriter resource
* @param $empty (optional) bool
*/
function xmlwriter_flush($xmlwriter, $empty);
/**
* Create new xmlwriter using memory for string output
* @return resource
* @version PECL
*/
function xmlwriter_open_memory();
/**
* Create new xmlwriter using source uri for output
* @return resource
* @version PECL
* @param $source string
*/
function xmlwriter_open_uri($source);
/**
* Output current buffer as string
* @return string
* @version PECL
* @param $xmlwriter resource
* @param $flush (optional) bool
*/
function xmlwriter_output_memory($xmlwriter, $flush);
/**
* Toggle indentation on/off - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $indent bool
*/
function xmlwriter_set_indent($xmlwriter, $indent);
/**
* Set string used for indenting - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $indentString string
*/
function xmlwriter_set_indent_string($xmlwriter, $indentString);
/**
* Create start attribute - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
*/
function xmlwriter_start_attribute($xmlwriter, $name);
/**
* Create start namespaced attribute - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $prefix string
* @param $name string
* @param $uri string
*/
function xmlwriter_start_attribute_ns($xmlwriter, $prefix, $name, $uri);
/**
* Create start CDATA tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_start_cdata($xmlwriter);
/**
* Create start comment - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
*/
function xmlwriter_start_comment($xmlwriter);
/**
* Create document tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $version (optional) string
* @param $encoding (optional) string
* @param $standalone (optional) string
*/
function xmlwriter_start_document($xmlwriter, $version, $encoding, $standalone);
/**
* Create start DTD tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
* @param $pubid (optional) string
* @param $sysid (optional) string
*/
function xmlwriter_start_dtd($xmlwriter, $name, $pubid, $sysid);
/**
* Create start DTD element - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
*/
function xmlwriter_start_dtd_element($xmlwriter, $name);
/**
* Create start element tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
*/
function xmlwriter_start_element($xmlwriter, $name);
/**
* Create start namespaced element tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $prefix string
* @param $name string
* @param $uri string
*/
function xmlwriter_start_element_ns($xmlwriter, $prefix, $name, $uri);
/**
* Create start PI tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $target string
*/
function xmlwriter_start_pi($xmlwriter, $target);
/**
* Write text - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $content string
*/
function xmlwriter_text($xmlwriter, $content);
/**
* Write full attribute - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
* @param $content string
*/
function xmlwriter_write_attribute($xmlwriter, $name, $content);
/**
* Write full CDATA tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $content string
*/
function xmlwriter_write_cdata($xmlwriter, $content);
/**
* Write full comment tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $content string
*/
function xmlwriter_write_comment($xmlwriter, $content);
/**
* Write full DTD tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
* @param $pubid (optional) string
* @param $sysid (optional) string
* @param $subset (optional) string
*/
function xmlwriter_write_dtd($xmlwriter, $name, $pubid, $sysid, $subset);
/**
* Write full element tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $name string
* @param $content string
*/
function xmlwriter_write_element($xmlwriter, $name, $content);
/**
* Write full namesapced element tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $prefix string
* @param $name string
* @param $uri string
* @param $content string
*/
function xmlwriter_write_element_ns($xmlwriter, $prefix, $name, $uri, $content);
/**
* Write full PI tag - returns FALSE on error
* @return bool
* @version PECL
* @param $xmlwriter resource
* @param $target string
* @param $content string
*/
function xmlwriter_write_pi($xmlwriter, $target, $content);
/**
* Get XML parser error string
* @return string
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $code int
*/
function xml_error_string($code);
/**
* Get current byte index for an XML parser
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
*/
function xml_get_current_byte_index($parser);
/**
* Get current column number for an XML parser
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
*/
function xml_get_current_column_number($parser);
/**
* Get current line number for an XML parser
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
*/
function xml_get_current_line_number($parser);
/**
* Get XML parser error code
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
*/
function xml_get_error_code($parser);
/**
* Start parsing an XML document
* @return int
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $data string
* @param $is_final (optional) bool
*/
function xml_parse($parser, $data, $is_final);
/**
* Create an XML parser
* @return resource
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $encoding string
*/
function xml_parser_create($encoding);
/**
* Create an XML parser with namespace support
* @return resource
* @version PHP 4 >= 4.0.5, PHP 5
* @param $encoding string
* @param $separator (optional) string
*/
function xml_parser_create_ns($encoding, $separator);
/**
* Free an XML parser
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
*/
function xml_parser_free($parser);
/**
* Get options from an XML parser
* @return mixed
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $option int
*/
function xml_parser_get_option($parser, $option);
/**
* Set options in an XML parser
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $option int
* @param $value mixed
*/
function xml_parser_set_option($parser, $option, $value);
/**
* Parse XML data into an array structure
* @return int
* @version PHP 3 >= 3.0.8, PHP 4, PHP 5
* @param $parser resource
* @param $data string
* @param &$values array
* @param &$index (optional) array
*/
function xml_parse_into_struct($parser, $data, &$values, &$index);
/**
* Set up character data handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_character_data_handler($parser, $handler);
/**
* Set up default handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_default_handler($parser, $handler);
/**
* Set up start and end element handlers
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $start_element_handler callback
* @param $end_element_handler callback
*/
function xml_set_element_handler($parser, $start_element_handler, $end_element_handler);
/**
* Set up end namespace declaration handler
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_end_namespace_decl_handler($parser, $handler);
/**
* Set up external entity reference handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_external_entity_ref_handler($parser, $handler);
/**
* Set up notation declaration handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_notation_decl_handler($parser, $handler);
/**
* Use XML Parser within an object
* @return bool
* @version PHP 4, PHP 5
* @param $parser resource
* @param &$object object
*/
function xml_set_object($parser, &$object);
/**
* Set up processing instruction (PI) handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_processing_instruction_handler($parser, $handler);
/**
* Set up start namespace declaration handler
* @return bool
* @version PHP 4 >= 4.0.5, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_start_namespace_decl_handler($parser, $handler);
/**
* Set up unparsed entity declaration handler
* @return bool
* @version PHP 3 >= 3.0.6, PHP 4, PHP 5
* @param $parser resource
* @param $handler callback
*/
function xml_set_unparsed_entity_decl_handler($parser, $handler);
/**
* Evaluates the XPath Location Path in the given string
* @return class
* @version PHP 4 >= 4.0.4, PECL
* @param $xpath_expression string
* @param $contextnode (optional) domnode
*/
function xpath_eval($xpath_expression, $contextnode);
/**
* Evaluates the XPath Location Path in the given string
* @return class
* @version PHP 4 >= 4.0.4, PECL
* @param $expression string
* @param $contextnode (optional) domnode
*/
function xpath_eval_expression($expression, $contextnode);
/**
* Creates new xpath context
* @return XPathContext
* @version PHP 4 >= 4.0.4, PECL
* @param $dom_document domdocument
*/
function xpath_new_context($dom_document);
/**
* Register the given namespace in the passed XPath context
* @return bool
* @version PHP 4 >= 4.2.0, PECL
* @param $xpath_context XPathContext
* @param $prefix string
* @param $uri string
*/
function xpath_register_ns($xpath_context, $prefix, $uri);
/**
* Register the given namespace in the passed XPath context
* @return bool
* @version PECL
* @param $xpath_context XPathContext
* @param $context_node (optional) object
*/
function xpath_register_ns_auto($xpath_context, $context_node);
/**
* Evaluate the XPtr Location Path in the given string
* @return class
* @version PHP 4 >= 4.0.4, PECL
* @param $eval_str string
* @param $contextnode (optional) domnode
*/
function xptr_eval($eval_str, $contextnode);
/**
* Create new XPath Context
* @return XPathContext
* @version PHP 4 >= 4.0.4, PECL
*/
function xptr_new_context();
/**
* Returns the information on the compilation settings of the backend
* @return string
* @version PHP 4 >= 4.3.0, PECL
*/
function xslt_backend_info();
/**
* Returns the name of the backend
* @return string
* @version PHP 4 >= 4.3.0, PECL
*/
function xslt_backend_name();
/**
* Returns the version number of Sablotron
* @return string
* @version PHP 4 >= 4.3.0, PECL
*/
function xslt_backend_version();
/**
* Create a new XSLT processor
* @return resource
* @version PHP 4 >= 4.0.3, PECL
*/
function xslt_create();
/**
* Returns an error number
* @return int
* @version PHP 4 >= 4.0.3, PECL
* @param $xh resource
*/
function xslt_errno($xh);
/**
* Returns an error string
* @return string
* @version PHP 4 >= 4.0.3, PECL
* @param $xh resource
*/
function xslt_error($xh);
/**
* Free XSLT processor
* @return 
* @version PHP 4 >= 4.0.3, PECL
* @param $xh resource
*/
function xslt_free($xh);
/**
* Get options on a given xsl processor
* @return int
* @version PHP 4 >= 4.3.0, PECL
* @param $processor resource
*/
function xslt_getopt($processor);
/**
* Perform an XSLT transformation
* @return mixed
* @version PHP 4 >= 4.0.3, PECL
* @param $xh resource
* @param $xmlcontainer string
* @param $xslcontainer string
* @param $resultcontainer (optional) string
* @param $arguments (optional) array
* @param $parameters (optional) array
*/
function xslt_process($xh, $xmlcontainer, $xslcontainer, $resultcontainer, $arguments, $parameters);
/**
* Set options on a given xsl processor
* @return mixed
* @version PHP 4 >= 4.3.0, PECL
* @param $processor resource
* @param $newmask int
*/
function xslt_setopt($processor, $newmask);
/**
* Set the base URI for all XSLT transformations
* @return 
* @version PHP 4 >= 4.0.5, PECL
* @param $xh resource
* @param $uri string
*/
function xslt_set_base($xh, $uri);
/**
* Set the encoding for the parsing of XML documents
* @return 
* @version PHP 4 >= 4.0.5, PECL
* @param $xh resource
* @param $encoding string
*/
function xslt_set_encoding($xh, $encoding);
/**
* Set an error handler for a XSLT processor
* @return 
* @version PHP 4 >= 4.0.4, PECL
* @param $xh resource
* @param $handler mixed
*/
function xslt_set_error_handler($xh, $handler);
/**
* Set the log file to write log messages to
* @return 
* @version PHP 4 >= 4.0.6, PECL
* @param $xh resource
* @param $log (optional) mixed
*/
function xslt_set_log($xh, $log);
/**
* Sets the object in which to resolve callback functions
* @return bool
* @version PHP 4 >= 4.3.0, PECL
* @param $processor resource
* @param &$obj object
*/
function xslt_set_object($processor, &$obj);
/**
* Set SAX handlers for a XSLT processor
* @return 
* @version 4.0.3 - 4.0.6 only
* @param $xh resource
* @param $handlers array
*/
function xslt_set_sax_handler($xh, $handlers);
/**
* Set the SAX handlers to be called when the XML document gets processed
* @return 
* @version PHP 4 >= 4.0.6, PECL
* @param $processor resource
* @param $handlers array
*/
function xslt_set_sax_handlers($processor, $handlers);
/**
* Set Scheme handlers for a XSLT processor
* @return 
* @version 4.0.5 - 4.0.6 only
* @param $xh resource
* @param $handlers array
*/
function xslt_set_scheme_handler($xh, $handlers);
/**
* Set the scheme handlers for the XSLT processor
* @return 
* @version PHP 4 >= 4.0.6, PECL
* @param $processor resource
* @param $handlers array
*/
function xslt_set_scheme_handlers($processor, $handlers);
/**
* Returns additional error information
* @return string
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
*/
function yaz_addinfo($id);
/**
* Configure CCL parser
* @return 
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
* @param $config array
*/
function yaz_ccl_conf($id, $config);
/**
* Invoke CCL Parser
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
* @param $query string
* @param &$result array
*/
function yaz_ccl_parse($id, $query, &$result);
/**
* Close YAZ connection
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
*/
function yaz_close($id);
/**
* Prepares for a connection to a Z39.50 server
* @return mixed
* @version PHP 4 >= 4.0.1, PECL
* @param $zurl string
* @param $options (optional) mixed
*/
function yaz_connect($zurl, $options);
/**
* Specifies the databases within a session
* @return bool
* @version PHP 4 >= 4.0.6, PECL
* @param $id resource
* @param $databases string
*/
function yaz_database($id, $databases);
/**
* Specifies Element-Set Name for retrieval
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $elementset string
*/
function yaz_element($id, $elementset);
/**
* Returns error number
* @return int
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
*/
function yaz_errno($id);
/**
* Returns error description
* @return string
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
*/
function yaz_error($id);
/**
* Prepares for an Extended Service Request
* @return 
* @version PECL
* @param $id resource
* @param $type string
* @param $args array
*/
function yaz_es($id, $type, $args);
/**
* Inspects Extended Services Result
* @return array
* @version PHP 4 >= 4.2.0, PECL
* @param $id resource
*/
function yaz_es_result($id);
/**
* Returns value of option for connection
* @return string
* @version PECL
* @param $id resource
* @param $name string
*/
function yaz_get_option($id, $name);
/**
* Returns number of hits for last search
* @return int
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $searchresult (optional) array
*/
function yaz_hits($id, $searchresult);
/**
* Prepares for Z39.50 Item Order with an ILL-Request package
* @return 
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
* @param $args array
*/
function yaz_itemorder($id, $args);
/**
* Prepares for retrieval (Z39.50 present)
* @return bool
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
*/
function yaz_present($id);
/**
* Specifies a range of records to retrieve
* @return 
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $start int
* @param $number int
*/
function yaz_range($id, $start, $number);
/**
* Returns a record
* @return string
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $pos int
* @param $type string
*/
function yaz_record($id, $pos, $type);
/**
* Prepares for a scan
* @return 
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
* @param $type string
* @param $startterm string
* @param $flags (optional) array
*/
function yaz_scan($id, $type, $startterm, $flags);
/**
* Returns Scan Response result
* @return array
* @version PHP 4 >= 4.0.5, PECL
* @param $id resource
* @param &$result (optional) array
*/
function yaz_scan_result($id, &$result);
/**
* Specifies schema for retrieval
* @return 
* @version PHP 4 >= 4.2.0, PECL
* @param $id resource
* @param $schema string
*/
function yaz_schema($id, $schema);
/**
* Prepares for a search
* @return bool
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $type string
* @param $query string
*/
function yaz_search($id, $type, $query);
/**
* Sets one or more options for connection
* @return 
* @version PECL
* @param $id resource
* @param $name string
* @param $value string
*/
function yaz_set_option($id, $name, $value);
/**
* Sets sorting criteria
* @return 
* @version PHP 4 >= 4.1.0, PECL
* @param $id resource
* @param $criteria string
*/
function yaz_sort($id, $criteria);
/**
* Specifies the preferred record syntax for retrieval
* @return 
* @version PHP 4 >= 4.0.1, PECL
* @param $id resource
* @param $syntax string
*/
function yaz_syntax($id, $syntax);
/**
* Wait for Z39.50 requests to complete
* @return mixed
* @version PHP 4 >= 4.0.1, PECL
* @param &$options array
*/
function yaz_wait(&$options);
/**
* Traverse the map and call a function on each entry
* @return 
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
* @param $callback string
*/
function yp_all($domain, $map, $callback);
/**
* Return an array containing the entire map
* @return array
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
*/
function yp_cat($domain, $map);
/**
* Returns the error code of the previous operation
* @return int
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
*/
function yp_errno();
/**
* Returns the error string associated with the given error code
* @return string
* @version PHP 4 >= 4.0.6, PHP 5 <= 5.0.4
* @param $errorcode int
*/
function yp_err_string($errorcode);
/**
* Returns the first key-value pair from the named map
* @return array
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
*/
function yp_first($domain, $map);
/**
* Fetches the machine's default NIS domain
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
*/
function yp_get_default_domain();
/**
* Returns the machine name of the master NIS server for a map
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
*/
function yp_master($domain, $map);
/**
* Returns the matched line
* @return string
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
* @param $key string
*/
function yp_match($domain, $map, $key);
/**
* Returns the next key-value pair in the named map
* @return array
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
* @param $key string
*/
function yp_next($domain, $map, $key);
/**
* Returns the order number for a map
* @return int
* @version PHP 3 >= 3.0.7, PHP 4, PHP 5 <= 5.0.4
* @param $domain string
* @param $map string
*/
function yp_order($domain, $map);
/**
* Gets the Zend guid
* @return string
* @version PHP 4, PHP 5
*/
function zend_logo_guid();
/**
* Gets the version of the current Zend engine
* @return string
* @version PHP 4, PHP 5
*/
function zend_version();
/**
* Close a ZIP file archive
* @return 
* @version PHP 4 >= 4.1.0, PECL
* @param $zip resource
*/
function zip_close($zip);
/**
* Close a directory entry
* @return 
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
*/
function zip_entry_close($zip_entry);
/**
* Retrieve the compressed size of a directory entry
* @return int
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
*/
function zip_entry_compressedsize($zip_entry);
/**
* Retrieve the compression method of a directory entry
* @return string
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
*/
function zip_entry_compressionmethod($zip_entry);
/**
* Retrieve the actual file size of a directory entry
* @return int
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
*/
function zip_entry_filesize($zip_entry);
/**
* Retrieve the name of a directory entry
* @return string
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
*/
function zip_entry_name($zip_entry);
/**
* Open a directory entry for reading
* @return bool
* @version PHP 4 >= 4.1.0, PECL
* @param $zip resource
* @param $zip_entry resource
* @param $mode (optional) string
*/
function zip_entry_open($zip, $zip_entry, $mode);
/**
* Read from an open directory entry
* @return string
* @version PHP 4 >= 4.1.0, PECL
* @param $zip_entry resource
* @param $length (optional) int
*/
function zip_entry_read($zip_entry, $length);
/**
* Open a ZIP file archive
* @return resource
* @version PHP 4 >= 4.1.0, PECL
* @param $filename string
*/
function zip_open($filename);
/**
* Read next entry in a ZIP file archive
* @return resource
* @version PHP 4 >= 4.1.0, PECL
* @param $zip resource
*/
function zip_read($zip);
/**
* Returns the coding type used for output compression
* @return string
* @version PHP 4 >= 4.3.2, PHP 5
*/
function zlib_get_coding_type();
?>
