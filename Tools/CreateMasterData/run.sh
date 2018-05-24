#/bin/sh

source ~/.bash_profile

#cd hogehoge


if [ -e deploy/ ]; then
    # 存在する場合
    echo "exist deploy"
else
    # 存在しない場合
    mkdir deploy
fi

# 本来はmysql2sqlite.txtでsliteファイルを作成する
# 仕向けごとにフォルダを生成する
rm deploy/*
rm out/*
rm -fr lib/__pycache__

## reviewはバージョンアップ待ち
#SERVER_TYPE=review
#SERVER_TYPE=stg0
#SERVER_TYPE=stg1
#SERVER_TYPE=stg2a
#SERVER_TYPE=stg2b
#SERVER_TYPE=stg2c
#SERVER_TYPE=stg3a
#SERVER_TYPE=stg3b
#SERVER_TYPE=stg3c
#SERVER_TYPE=dev0
SERVER_TYPE=dev1

#DATA_SERVER_TYPE=prod

DUMP_PATH=settings/mysqldump.sql
VERSION_TYPE=$(python GetApiVersion.py ${SERVER_TYPE})
echo "ServerAPI: ${VERSION_TYPE}"

#python GetSchema.py ${SERVER_TYPE} ${DUMP_PATH}

#cp settings/master.normal.bytes deploy/
chmod 755 mysql2sqlite.txt
./mysql2sqlite.txt ${DUMP_PATH} | sqlite3 deploy/master.normal.bytes
cp deploy/master.normal.bytes deploy/master.normal.zero.bytes

# サーバ切り替える対応がいる
python GetMaster2.py ${SERVER_TYPE} ${DATA_SERVER_TYPE}

# Sqliteのファイル生成（データ入り）
sqlite3 ./deploy/master.normal.bytes < ./settings/create_dump.sql
sqlcipher ./deploy/master.p2.bytes < ./settings/create_pass.sql

# Sqliteのファイル生成（データ空）
sqlite3 ./deploy/master.normal.zero.bytes < ./settings/create_dump_zero.sql
sqlcipher ./deploy/master.p2.zero.bytes < ./settings/create_pass_zero.sql

# Googleドライブで転送する
