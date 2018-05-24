# -*- coding: utf-8 -*-
import os
import sqlite3
import sys

masterdir = "out"


def tablePath(master):
    if os.path.exists(masterdir) == False:
        os.mkdir(masterdir)

    tablepath = masterdir + "/_table_" + master + ".txt"
    return tablepath


def filePath(master):
    if os.path.exists(masterdir) == False:
        os.mkdir(masterdir)

    filepath = masterdir + "/" + master + ".txt"
    return filepath


def replaceText(val):
    str_val = ""
    if type(val) == str:
        str_val = val
    else:
        str_val = str(val)

    # 改行コード
    str_val = str_val.replace('\r\n', '\r')
    str_val = str_val.replace('\n', '\r')
    str_val = str_val.replace('\r', '\\n')

    # '
    str_val = str_val.replace('\'', '\'\'')

    return str_val


def saveSchema(get_dict, jsonkey, master):
    upd_list = []
    tablepath = ""
    try:
        result = get_dict["result"]
        typeResult = result is None

        if typeResult == True:
            print("saveSchema skip jesonkey: " + jsonkey)
            return

        if isinstance(result.get(jsonkey), type(None)):
            print("saveSchema ng ng ng ng ng ng:")
            print("no jesonkey:")
            return

        upd_list = result[jsonkey]["upd_list"]
        tablepath = tablePath(master)
    except Exception as e:
        print(e.message)
        sys.exit("sys.exit saveSchema")
    finally:
        print("jsonkey  : " + jsonkey)
        print("master   : " + master)
        print("tablepath : " + tablepath)

    keylist = []

    for upd in upd_list:
        if len(keylist) == 0:
            keys = upd.keys()
            keylist = list(keys)
            last_key = keylist[-1]
            break

    fp = open(tablepath, 'w')
    for key in keylist:
        fp.write(key + "\n")
    fp.close()


def save(get_dict, jsonkey, master):
    result = get_dict["result"]
    typeResult = result is None

    if typeResult == True:
        print("saveSchema skip jesonkey: " + jsonkey)
        return

    if isinstance(result.get(jsonkey), type(None)):
        print("save ng ng ng ng ng ng:")
        print("no jesonkey:")
        print("jsonkey  : " + jsonkey)
        return

    upd_list = result[jsonkey]["upd_list"]

    filepath = filePath(master)

    print("jsonkey  : " + jsonkey)
    print("master   : " + master)
    print("filepath : " + filepath)

    fp = open(filepath, 'w')
    keylist = []

    for upd in upd_list:

        if len(keylist) == 0:
            keys = upd.keys()
            keylist = list(keys)
            last_key = keylist[-1]

        key_str = ""
        value_str = ""
        for key in keylist:
            key_str += key

            if last_key != key:
                key_str += ", "

            val = upd[key]
            val = replaceText(val)
            value_str += "'" + val + "'"

            if last_key != key:
                value_str += ", "

        sql = "insert into " + master + \
            " (" + key_str + ") values (" + value_str + "); \n"
        fp.write(sql)
        # print(sql)

    fp.close()


def tempsave(get_dict, master):
    masterdir = "dump"

    if os.path.exists(masterdir) == False:
        os.mkdir(masterdir)

    filepath = masterdir + "/" + master + ".txt"
    fp = open(filepath, 'w')
    fp.write(str(get_dict))
    fp.close()


def vacuum(dbpath):
    try:
        connect = sqlite3.connect(dbpath, isolation_level=None)
        connect.execute("VACUUM")
    except Exception:
        if connect:
            connect.rollback()
        sys.exit("sys.exit vacuum")
    finally:
        if connect:
            connect.commit()
            connect.close()


def removeTable(dbpath, table_list):
    print("removeTable: " + dbpath)
    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()
    cursor.execute("SELECT DISTINCT tbl_name FROM sqlite_master")
    tbl_name_list = cursor.fetchall()

    remove_table_list = []
    for item in tbl_name_list:
        table_name = item[0]
        remove_table_list.append(table_name)
        cursor.execute("DELETE FROM " + table_name)

    for table in table_list:
        try:
            remove_table_list.remove(table)
        except ValueError:
            print("pass: " + table)
            pass

    for table in remove_table_list:
        cursor.execute(
            "SELECT type, name FROM sqlite_master where tbl_name = '" + table + "'")
        remove_name_list = cursor.fetchall()
        for remove_name in remove_name_list:
            str_type = remove_name[0]
            if str_type == "table":
                cursor.execute("DROP TABLE IF EXISTS " + remove_name[1])
            elif str_type == "index":
                cursor.execute("DROP INDEX IF EXISTS " + remove_name[1])

    connect.commit()
    connect.close()
    vacuum(dbpath)


def createTable(dbpath, table_list):
    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()

    for table in table_list:
        base_colums = []

        filepath = tablePath(table)
        try:
            fp = open(filepath, 'r')
            for column in fp:
                column = column.replace('\n', '')
                base_colums.append(str(column))
        except Exception as e:
            print("filepath pass: " + filepath)
            pass
        finally:
            if fp:
                fp.close()

        if len(base_colums) == 0:
            continue

        cursor.execute("pragma table_info( '" + table + "' )")
        table_info = cursor.fetchall()
        # print(table_info)

        find_colums = []
        for info in table_info:
            find_info = str(info[1])
            # print ("find_info: " + find_info)
            if find_info in base_colums:
                find_colums.append(info)
            else:
                print("remove find_info: " + find_info)

        if len(find_colums) == 0:
            print("find_colums pass: " + table)
            continue

        primarykey = ""
        create_table = "CREATE TABLE \"" + table + "\" ("
        for data in find_colums:
            if int(data[0]) != 0:
                create_table += ", "

            create_table += data[1] + " " + data[2] + " "

            if int(data[3]) == 1:
                create_table += "NOT NULL "

            if str(data[4]) != "None":
                create_table += "DEFAULT " + str(data[4])

            if int(data[5]) == 1:
                primarykey = data[1]

        if len(primarykey) > 0:
            create_table += ",  PRIMARY KEY (\"fix_id\")"

        create_table += ")"
        print(create_table)
        create_index = []

        cursor.execute(
            "SELECT type, name, sql FROM sqlite_master where tbl_name = '" + table + "'")
        remove_name_list = cursor.fetchall()
        for remove_name in remove_name_list:
            str_type = remove_name[0]
            if str_type == "table":
                remove_table = "DROP TABLE IF EXISTS " + remove_name[1]
                print(remove_table)
                cursor.execute(remove_table)
            elif str_type == "index":
                if remove_name[2] == None:
                    # print("continue---- None")
                    continue
                create_index.append(remove_name[2])
                drop_index = "DROP INDEX IF EXISTS " + remove_name[1]
                # print("drop_index----")
                # print(drop_index)
                # print("remove_name[2]----")
                # print(remove_name[2])
                cursor.execute(drop_index)

        cursor.execute(create_table)

        for c_index in create_index:
            # print(c_index)
            cursor.execute(c_index)

    connect.commit()
    connect.close()

    vacuum(dbpath)


def createTableSetting(dbpath, settingfilepath):

    createCommand = []
    try:
        fp = open(settingfilepath, 'r')
        for command in fp:
            command = command.replace('\n', '')
            if(len(command) > 0):
                createCommand.append(str(command))
    except Exception as e:
        print("settingfilepath pass: " + settingfilepath)
        print(e.message)
        sys.exit("sys.exit createTableSetting")
    finally:
        if fp:
            fp.close()

    if len(createCommand) == 0:
        return

    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()

    for command in createCommand:
        print("add table: " + command)
        cursor.execute(command)

    connect.commit()
    connect.close()

    vacuum(dbpath)


def createIndexSetting(dbpath, settingfilepath):

    createCommand = []
    try:
        fp = open(settingfilepath, 'r')
        for command in fp:
            command = command.replace('\n', '')
            if(len(command) > 0):
                createCommand.append(str(command))
    except Exception as e:
        print("settingfilepath pass: " + settingfilepath)
        print(e.message)
        sys.exit("sys.exit createIndexSetting")
    finally:
        if fp:
            fp.close()

    if len(createCommand) == 0:
        return

    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()

    for command in createCommand:
        print("add index: " + command)
        cursor.execute(command)

    connect.commit()
    connect.close()

    vacuum(dbpath)


def insertTable(dbpath, table_list):
    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()

    for table in table_list:
        filepath = filePath(table)
        run_sql = ""
        try:
            fp = open(filepath, 'r')
        except Exception as e:
            print("filepath pass: " + filepath)
            if fp:
                fp.close()
            continue
            pass

        try:
            count = 0
            for sql in fp:
                run_sql = sql.replace('\\n', '\n')
                if len(run_sql) == 0:
                    continue
                cursor.execute(run_sql)
                count += 1

            print("insart : " + filepath + " | count = " + str(count))

        except Exception as e:
            print("filepath pass: " + filepath)
            print("run_sql: " + run_sql)
            print(e.message)
            sys.exit("sys.exit insertTable")
        finally:
            if fp:
                fp.close()

    connect.commit()
    connect.close()
    vacuum(dbpath)


def runSql(dbpath, run_sql):
    connect = sqlite3.connect(dbpath)
    cursor = connect.cursor()

    cursor.execute(run_sql)
    print("runSql insart : " + run_sql)

    connect.commit()
    connect.close()
    vacuum(dbpath)
