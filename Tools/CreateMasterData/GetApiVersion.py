# -*- coding: utf-8 -*-

import sys

from lib import ServerSeting

argvs = sys.argv  # コマンドライン引数を格納したリストの取得
argc = len(argvs)  # 引数の個数

if (argc != 2):   # 引数が足りない場合は、その旨を表示
    sys.exit('Usage: # python %s servertype' % argvs[0])

print(ServerSeting.ApiVersion(argvs[1]))
