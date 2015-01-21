#include <QDebug>
#include <QSettings>
#include <QDir>
#include <QProcess>
#include <QDesktopServices>
#include <QUrl>
#include "bridgeserver.h"
#include "bridgethread.h"

BridgeServer::BridgeServer(QObject *parent) : QTcpServer(parent)
{
    runningThreads = 0;
    QSettings settings;
    settings.beginGroup("server");
    qDebug() << "Connecting:" << settings.value("host").toString() << settings.value("port").toInt();
    QHostAddress host(settings.value("host").toString());
    listen(host, settings.value("port").toInt());
    qDebug() << (isListening() ? "Success" : "Failure");
}

void BridgeServer::notifyStatus()
{
    emit bridgeStatus(runningThreads, watched.keys().count());
}

void BridgeServer::incomingConnection(qintptr socketDescriptor)
{
    BridgeThread *thread = new BridgeThread(socketDescriptor, 0);
    connect(thread, SIGNAL(disconnected()), this, SLOT(threadDisconnected()));
    connect(thread, SIGNAL(command(QString,QString)), this, SLOT(command(QString,QString)));
    runningThreads++;
    notifyStatus();
}

void BridgeServer::command(QString cmd, QString value)
{
    BridgeThread *thread = (BridgeThread*)sender();
    qDebug() << cmd << value;
    if (cmd == "watch")
    {
        releaseHandler(thread);
        BridgeHandler *handler = createWatchHandler(value);
        handler->useCount++;
        thread->handler = handler;
        connect(handler, SIGNAL(notifyChanged(QString)), thread, SLOT(sendMessage(QString)));
    }
    else if (cmd == "unwatch") releaseHandler(thread);
    else if (cmd == "open") openDocument(value);
    else qDebug() << cmd << value;
}

void BridgeServer::openDocument(QString path)
{
    BridgeHandler handler;
    QString localPath = handler.getLocalPath(path.replace('\\', '/'));
    qDebug() << "Open:" << localPath;
    QFile file(localPath);
    if (file.exists()) QDesktopServices::openUrl(QUrl::fromLocalFile(localPath));
    else qDebug() << "File not found:" << path;
}

BridgeHandler* BridgeServer::createWatchHandler(QString path)
{
    BridgeHandler *handler;
    if (watched.contains(path)) handler = watched[path];
    else
    {
        handler = new BridgeHandler(this);
        handler->command("watch", path);
        watched[path] = handler;
        notifyStatus();
    }
    return handler;
}

void BridgeServer::threadDisconnected()
{
    BridgeThread *thread = (BridgeThread*)sender();
    qDebug() << "disconnected" << thread;
    releaseHandler(thread);
    thread->deleteLater();
    runningThreads--;
    notifyStatus();
}

void BridgeServer::releaseHandler(BridgeThread *thread)
{
    if (thread->handler != 0)
    {
        disconnect(thread->handler, SIGNAL(notifyChanged(QString)), thread, SLOT(sendMessage(QString)));
        if (--thread->handler->useCount <= 0) watched.remove(thread->handler->watched);
        thread->handler = 0;
    }
}
