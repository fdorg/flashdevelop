#ifndef BRIDGESERVER_H
#define BRIDGESERVER_H

#include <QTcpServer>
#include <QList>
#include "bridgethread.h"

class BridgeServer : public QTcpServer
{
    Q_OBJECT
    QHash<QString, BridgeHandler*> watched;
    int runningThreads;
    void releaseHandler(BridgeThread *thread);
    void openDocument(QString path);
    void notifyStatus();

public:
    BridgeServer(QObject *parent = 0);
    BridgeHandler *createWatchHandler(QString path);

protected:
    void incomingConnection(qintptr socketDescriptor);

signals:
    void bridgeStatus(int threads, int watchers);

private slots:
    void command(QString cmd, QString value);
    void threadDisconnected();
};

#endif
