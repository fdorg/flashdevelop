package;
import haxe.Json;
class OrganizeImportsIssue2512_2 {
    static function main() {
        trace("${Json.stringify({x:1})}");
    }
}