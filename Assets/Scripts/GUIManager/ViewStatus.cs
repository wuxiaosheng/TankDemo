using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewStatus
{
    private ViewType _status = ViewType.NONE;
    public ViewStatus() {
    }
    public void setStatus(ViewType status) {
        if (status == _status) { return; }
        if (_status == ViewType.NONE) {
            if (status == ViewType.LOGIN) {
                onLogin();
            }
        } else if (_status == ViewType.LOGIN) {
            if (status == ViewType.WAIT_JOIN) {
                onStatusExit(_status);
                onWaitJoin();
            }
        }
    }
    public ViewType getStatus() {
        return _status;
    }

    private void onStatusExit(ViewType status) {
        if (status == ViewType.LOGIN) {
            onLoginExit();
        } else if (status == ViewType.WAIT_JOIN) {
            onWaitJoinExit();
        }
    }

    private void onLoginExit() {
        ViewBase view = GUIManager.getInstance().getView("UILogin");
        if (view != null) {
            view.setVisible(false);
        }
    }

    private void onWaitJoinExit() {
        ViewBase waitting = GUIManager.getInstance().getView("UIWaitting");
        if (waitting != null) {
            waitting.setVisible(false);
        }
    }

    private void onLogin() {
        _status = ViewType.LOGIN;
        ViewBase view = GUIManager.getInstance().getView("UILogin");
        GameObject root = GUIManager.getInstance().getRoot();
        if (view == null) {
            UILogin login = new UILogin("UILogin", "Prefabs/View/UIPreLogin", root.transform);
            GUIManager.getInstance().addView("UILogin", login);
        } else {
            view.setPosition(new Vector3(0, 0, 0));
            view.setVisible(true);
        }

    }

    private void onWaitJoin() {
        _status = ViewType.WAIT_JOIN;
        ViewBase view = GUIManager.getInstance().getView("UIWaitting");
        GameObject root = GUIManager.getInstance().getRoot();
        if (view == null) {
            UIWaitting waitting = new UIWaitting("UIWaitting", "Prefabs/View/UIPreWaitting", root.transform);
            GUIManager.getInstance().addView("UIWaitting", waitting);
        } else {
            view.setPosition(new Vector3(0, 0, 0));
            view.setVisible(true);
        }
    }
}
