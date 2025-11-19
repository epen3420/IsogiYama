# build-unity.yml
Unityのビルドをする。Windows/WebGLのビルドに対応している。

### 選択可能なもの
* build_target_name: [Windows/WebGL]


# create-release-note.yml
GitHubのリリースノートを更新する。

### 選択可能なもの
* VERSION_TAG: [string]
* ARTIFACT_NAME: [string]
* (RUN_ID): [string]


# release-github.yml
ビルドをして、githubにリリースする

### 選択可能なもの
* build_target_name: [Windows/WebGL]
* version_tag: [string]


# event-handler.yml
主にPRのイベントを購読して処理をする
