@echo off
sc.exe stop AnyeSoftAutoUpdateServerService
sc.exe delete AnyeSoftAutoUpdateServerService
@pause