
:: http://cesanta.com/docs/Embed.shtml

set FLAGS=-DMONGOOSE_NO_DAV -DMONGOOSE_NO_CGI -DMONGOOSE_NO_AUTH -DMONGOOSE_NO_LOGGING

mkdir obj
cl mongoose_server.c mongoose.c %FLAGS% /TC /MD /Fo.\obj\

