#ifndef FILESYSTEMWATCHEREX_H
#define FILESYSTEMWATCHEREX_H

#include <QObject>
#include <QFileInfo>
#include <QDateTime>
#include <QFileSystemWatcher>

class WatchedFile
{
public:
    WatchedFile(QFileInfo info)
    {
        file = info.absoluteFilePath();
        exists = info.exists();
        if (exists) lastModified = info.lastModified();
    }
    QString file;
    QDateTime lastModified;
    bool exists;
};

class FileSystemWatcherEx : public QObject
{
    Q_OBJECT
    QStringList subFoldersList(QString folder);
    QFileSystemWatcher *fsw;
    QString basePath;
    bool includeSubs;
    WatchedFile *watchedFile;

public:
    explicit FileSystemWatcherEx(QObject *parent = 0);
    ~FileSystemWatcherEx();
    QString path();
    void setPath(QString path, bool includeSubdirectories);
    void setFile(QString path);

signals:
    void fileSystemChanged(QString path);

private slots:
    void directoryChanged(QString path);
    void fileChanged(QString path);
};

#endif // FILESYSTEMWATCHEREX_H
