# -*- coding: utf-8 -*-
from enum import IntEnum

version400 = "4.0.0"
version500 = "5.0.0"
version501 = "5.0.1"
version502 = "5.0.2"
version510 = "5.1.0"
version520 = "5.2.0"
version530 = "5.3.0"
version535 = "5.3.5"
version540 = "5.4.0"
version550 = "5.5.0"


class types(IntEnum):
    prod = 0,
    review = 1,
    stg0 = 2,
    stg1 = 3,
    stg2a = 4,
    stg2b = 5,
    stg2c = 6,
    stg3a = 7,
    stg3b = 8,
    stg3c = 9,
    dev0 = 10,
    dev1 = 11,
    local = 100,  # local


def UUID(servername):
    uuid = ""

    if servername == "prod":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "review":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg0":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg1":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg2a":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg2b":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg2c":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg3a":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg3b":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "stg3c":
        uuid = "00000000-0000-0000-0000-000000000000"
    elif servername == "dev0":
        uuid = "00000000-0000-0000-0000-000000000000"
    else:
        # dev1
        uuid = "aaeda634-0b37-4b17-84cb-532b39100e22"

    return uuid


def ApiVersion(servername):
    api_version = ""

    if servername == "prod":
        api_version = version540
    elif servername == "review":
        api_version = version550
    elif servername == "stg0":
        api_version = version540
    elif servername == "stg1":
        api_version = version540
    elif servername == "stg2a":
        api_version = version550
    elif servername == "stg2b":
        api_version = version550
    elif servername == "stg2c":
        api_version = version550
    elif servername == "stg3a":
        api_version = version550
    elif servername == "stg3b":
        api_version = version550
    elif servername == "stg3c":
        api_version = version550
    elif servername == "dev0":
        api_version = version540
    else:
        # dev1
        api_version = version550

    return api_version


def ApiUrl(servername):
    api_url = ""

    if servername == "prod":
        api_url = "https://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "review":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg0":  #
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg1":  #
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg2a":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg2b":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg2c":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg3a":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg3b":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "stg3c":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    elif servername == "dev0":
        api_url = "http://example.com" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"
    else:
        # dev1
        api_url = "http://104.238.161.24" + "/pqdm/" + \
            ApiVersion(servername) + "/api/"

    return api_url


def Sqlite3Path(servername):
    sqlite3_filepath = "deploy/master.normal.bytes"

    return sqlite3_filepath


def Sqlite3ZeroPath(servername):
    sqlite3_filepath = "deploy/master.normal.zero.bytes"

    return sqlite3_filepath


def CreateTable(servername):
    createtable_filepath = ""

    if servername == "prod":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "review":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg0":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg1":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg2a":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg2b":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg2c":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg3a":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg3b":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "stg3c":
        createtable_filepath = "settings/createtables.txt"
    elif servername == "dev0":
        createtable_filepath = "settings/createtables.txt"
    else:
        # dev1
        createtable_filepath = "settings/createtables.txt"

    return createtable_filepath


def CreateIndex(servername):
    createindex_filepath = ""

    if servername == "prod":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "review":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg0":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg1":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg2a":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg2b":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg2c":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg3a":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg3b":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "stg3c":
        createindex_filepath = "settings/createindex.txt"
    elif servername == "dev0":
        createindex_filepath = "settings/createindex.txt"
    else:
        # dev1
        createindex_filepath = "settings/createindex.txt"

    return createindex_filepath


def SelectServerType(servername):

    selectserver = types.prod.value

    if servername == "prod":
        selectserver = types.prod.value
    elif servername == "review":
        selectserver = types.review.value
    elif servername == "stg0":
        selectserver = types.stg0.value
    elif servername == "stg1":
        selectserver = types.stg1.value
    elif servername == "stg2a":
        selectserver = types.stg2a.value
    elif servername == "stg2b":
        selectserver = types.stg2b.value
    elif servername == "stg2c":
        selectserver = types.stg2c.value
    elif servername == "stg3a":
        selectserver = types.stg3a.value
    elif servername == "stg3b":
        selectserver = types.stg3b.value
    elif servername == "stg3c":
        selectserver = types.stg3c.value
    elif servername == "dev0":
        selectserver = types.dev0.value
    else:
        # dev1
        selectserver = types.dev1.value

    return selectserver
