# Anye.Soft.AutoUpdate

#### 介绍
暗夜软件自动更新(.net5)
差异更新,自动生成配置文件

#### 软件架构
Grpc通讯
Topshelf(用于服务端)
Stylet(用于更新管理[wpf程序])
PropertyChanged.Fody(用于更新管理[wpf程序])

#### 安装教程

1.  先运行服务端
    a.双击直接运行:Anye.Soft.AutoUpdateServer.exe
    b.以系统服务运行:右键[以管理员身份运行]运行[1_install.bat]即可,启动服务请执行[2_start_service.bat],停止服务请执行[3_stop_service.bat],卸载服务请执行[4_uninstall.bat]
2.  打开更新管理程序:Anye.Soft.AutoUpdateSetting.exe,首次会自动生成配置文件,生成后即可使用自动更新程序
3.  执行自动更新客户端,即可自动升级,具体配置可看AutoUpdate.json;或者执行[自动升级.bat],也可以自动下载需要升级的程序

#### 使用说明

1.  服务端配置文件说明:
        设置配置文件:AutoUpdateSetting.json
        				UpdatePath-------更新根目录,默认值:Clients
        				FileName---------版本配置文件,默认值:AutoUpdateConfig.json
        
        [版本目录]名按整数数值命名,数值越大,版本越新;
        [版本配置文件]:AutoUpdateConfig.json(每个版本目录下的文件)
        
        文件结构:
        	Clients-------------根目录
        		App1---------------客户端目录
        			1--------------版本目录
        			2--------------版本目录
        		App2---------------客户端目录
        			1--------------版本目录
        
        更新管理界面,首次会自动生成[版本配置文件],另外更新管理界面只会加载最新版本和前一版本
2.  客户端配置文件说明(AutoUpdate.json):
        {
          "IsUseAutoUpdateJson": true,//true,使用此配置文件来运行(自动更新客户端),false使用脚本传参数方式运行
          "ClientName": "App1",//要更新的客户端名称,对应服务端的[客户端目录]名称
          "ClientVersion": 0,//要更新的客户端版本,对应服务端的[版本目录]名称
          "UpdatePath": "..\\Client",//要更新的客户端路径,仅支持相对路径
          "FileName": "AnyeSoft.Client",//用于关闭和启动客户端程序的名称(普通程序或服务程序)
          "EnumAutoUpdate": 0,//0普通程序,1服务程序
          "IsConsoleReadKey": true//true等待用户按键后关闭程序,false执行完直接关闭
        }
3.  xxxx

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
