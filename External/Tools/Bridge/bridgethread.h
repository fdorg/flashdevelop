#ifndef BRIDGETHREAD_H
#define BRIDGETHREAD_H

#include <QThread>
#include <QTcpSocket>
#include <QMutex>
#include <QTimer>
#include <QStringList>
#include "filesystemwatcherex.h"

class BridgeHandler : public QObject
{
    Q_OBJECT
    QString remotePath;
    QStringList filter;
    bool isSpecial;
    QString special;
    QString localPath;
    QString watchedPath;
    FileSystemWatcherEx *fsw;
    void watchPath(QString param);
    QString getSpecialPath();
    QString getRemotePath(QString path);
    void copyPatched(QString filePath, QString destPath);

public:
    BridgeHandler(QObject *parent = 0);
    ~BridgeHandler();
    QString watched;
    int useCount;
    QString getLocalPath(QString path);

signals:
    void notifyChanged(QString path);

public slots:
    void command(QString name, QString param);

private slots:
    void localChanged(QString localPath);
};

class BridgeThread : public QObject
{
    Q_OBJECT
    int socketDescriptor;
    QTcpSocket *client;
    QTimer timer;
    QStringList queue;
    QMutex mutex;

public:
    BridgeThread(int descriptor, QObject *parent);
    ~BridgeThread();
    BridgeHandler *handler;
    static bool pathNotified;

signals:
    void disconnected();
    void command(QString name, QString param);

public slots:
    void sendMessage(QString message);

private slots:
    void client_disconnected();
    void client_readyRead();
    void timer_elapsed();

protected:
    void run();
};

#endif
