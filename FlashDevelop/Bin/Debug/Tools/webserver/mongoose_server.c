#include "mongoose.h"

int main(int argc, char *argv[]) {

	char* port = "2000";
	struct mg_server *server = mg_create_server(NULL, NULL);
	
	if (argc > 1) port = argv[1];
	printf("\nHttp server listening on port %s\n", port);

	mg_set_option(server, "document_root", "."); // Serve current directory
	mg_set_option(server, "listening_port", port);

	for (;;) {
		mg_poll_server(server, 1000);   // Infinite loop, Ctrl-C to stop

		if (mg_get_listening_socket(server) == -1) { // unable to connect
			_sleep(500);
		}
	}
	mg_destroy_server(&server);

	return 0;
}