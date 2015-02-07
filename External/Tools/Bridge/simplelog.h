#ifndef SIMPLELOG_H
#define SIMPLELOG_H

#include <fstream>
#include <QDebug>
#include <QTime>

using namespace std;
ofstream logfile;

void SimpleLogHandler(QtMsgType type, const QMessageLogContext&, const QString& msg)
{
    switch (type)
    {
        case QtDebugMsg:
            logfile << QTime::currentTime().toString().toLatin1().data() << " : " << msg.toLatin1().data() << "\n";
            break;
        case QtCriticalMsg:
            logfile << QTime::currentTime().toString().toLatin1().data() << " Critical: " << msg.toLatin1().data() << "\n";
            break;
        case QtWarningMsg:
            logfile << QTime::currentTime().toString().toLatin1().data() << " Warning: " << msg.toLatin1().data() << "\n";
            break;
        case QtFatalMsg:
            logfile << QTime::currentTime().toString().toLatin1().data() <<  " Fatal: " << msg.toLatin1().data() << "\n";
            abort();
    }
    logfile.flush();
}

#endif // SIMPLELOG_H
