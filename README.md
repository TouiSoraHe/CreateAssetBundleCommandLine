# CreateAssetBundleCommandLine
### 项目说明:

------------

该项目为一个Unity项目,主要提供命令行进行AssetBundle打包的功能.

### Assets目录说明:

------------

**Editor**:该目录下包含了入口脚本,主要提供命令行调用的入口,以及相关的逻辑操作

**Resources**:该目录为一个临时目录,每次在打包前将会清空该目录,然后会将资源导入到该目录,打包生成的文件也会输出到该目录的**Temp**目录下

**StreamingAssets**:该目录下放置了各种模板文件(json),模板文件的详细信息将会在后面提到

**Script**:该目录下包含了各类C#脚本,主要为打包提供工具服务

### 功能说明:

------------

1. 提供AssetBundle打包的功能
1. 提供生成sdf文件的功能 (sdf文件:用来描述AssetBundle包的内部结构信息)
1. 提供选择打包平台的功能
1. 生成详细日志信息
1. 打包前对资源进行各项设置(动画类型,贴图类型,材质等等)

### 使用说明:
通过以下CMD命令来进行调用:
***UnityPath*** -quit -batchmode -nographics -logFile ***LogFilePath***  -projectPath ***ProjectPath***  -executeMethod ***ClassAndMethodName***  ***SrcPath***  ***OutPath*** ***ConfigJson***

**参数说明:**

**UnityPath**:Unity.exe的绝对路径,必选

**LogFilePath**:输出的日志文件的路径(该日志文件由Unity输出),必选(占位)

**ProjectPath**:本项目的绝对路径,必选

**ClassAndMethodName**:需要执行的脚本,只有两个值:CreateAssetBundle.MainMethod和CreateSDF.MainMethod,前者用来AssetBundle打包,后者用来创建sdf文件

**SrcPath**:需要打包的资源文件的绝对路径(包含文件名),必选

**OutPath**:打包完成后的输出路径(包含文件名),必选

**ConfigJson**:打包相关的配置信息,扩展使用,可选


### 补充说明:
**关于ConfigJson:**

ConfigJson作为扩展参数而存在,该参数是一个Json字符串,该json对象目前结构如下:





    {
        "ModelConfig":{
            "ModelAnimationType":"",
            "MaterialTypes":[
    
            ]
        },
        "TextureConfig":{
            "TextureType":"",
            "MaxTextureSize":-1,
            "WrapMode":"",
            "FilterMode":""
        }
    }

关于该json的更多说明,请查看**ConfigJson.cs**脚本的定义,该脚本位于**Assets/Script/Tools**中

**关于日志文件:**

本项目共存在两个日志文件.分别是:
1. **Unity命令行日志**:该日志由Unity自动产生,由于无法通过命令行返回错误信息,所有的Debug信息也全部都在该日志文件中,所以将错误信息加上特定的前缀"!@#$%^&*()"和后缀"!@#$%^&*()"作为标识(不包含双引号),然后通过对该日志文件进行正则表达式匹配来达到间接的传递参数!
1. **该项目本身的日志**:该日志文件位于项目根目录下(定义在**GlobalConstants**脚本中),该日志文件使用追加的方式来持续记录每一次打包的详细信息,为提供Debug信息!

**关于打包平台:**

请不要直接在代码中修改打包平台,应该通过配置文件中来进行设置,该配置文件位于项目目录的同级目录下,文件名为**project.config**,是一个json文件.

**关于同时打包:**

由于一个Unity项目同时只能被一个Unity打开,所以想要同时进行多个资源的打包,可以考虑将该项目拷贝多份
