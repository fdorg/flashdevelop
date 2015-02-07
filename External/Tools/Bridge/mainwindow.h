#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QtGui>
#include <QSystemTrayIcon>
#include <QLabel>
#include "bridgeserver.h"
#include "filesystemwatcherex.h"

namespace Ui
{
    class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

private slots:
    void quit();
    void showWindow();
    void bridgeStatus(int threads, int watchers);
    void on_btAdd_clicked();
    void on_btRemove_clicked();
    void on_listMap_itemSelectionChanged();
    void on_listMap_cellChanged(int row, int column);

protected:
    void closeEvent(QCloseEvent *event);

private:
    Ui::MainWindow *ui;
    QSystemTrayIcon *sti;
    BridgeServer *server;
    bool lockSettings;
    bool showOnStart;
    QLabel *statusLabel;
    QStatusBar *statusBar;
    void initTray();
    void initStatusBar();
    void initMapping();
    void initServer();
    void updateSettings();
};

#endif // MAINWINDOW_H
