#include <QtGui/QApplication>
#include "mainwindow.h"

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);

    QCoreApplication::setApplicationName("Bridge");
    QCoreApplication::setOrganizationDomain("flashdevelop.org");
    QCoreApplication::setOrganizationName("FlashDevelop");

    MainWindow w;
    //w.show();

    return a.exec();
}
