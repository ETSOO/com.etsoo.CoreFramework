using com.etsoo.Localization.Chinese;

namespace com.etsoo.Localization
{
    /// <summary>
    /// Chinese characters
    /// 中文字符
    /// </summary>
    internal static partial class ChineseCharacters
    {
        /// <summary>
        /// All Chinese characters 3
        /// 所有中文字符3
        /// </summary>
        public static readonly SortedDictionary<char, ChineseCharacter> Characters3 = new()
        {
            // 净: jing
            ['净'] = new()
            {
                PinYins = [new("jing", CharTone.Fourth)]
            },

            // 盲: mang
            ['盲'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 放: fang
            ['放'] = new()
            {
                PinYins = [new("fang", CharTone.Fourth)]
            },

            // 刻: ke
            ['刻'] = new()
            {
                PinYins = [new("ke", CharTone.Fourth)]
            },

            // 育: yu
            ['育'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 闸: zha
            ['闸'] = new()
            {
                PinYins = [new("zha", CharTone.Second)]
            },

            // 闹: nao
            ['闹'] = new()
            {
                PinYins = [new("nao", CharTone.Fourth)]
            },

            // 郑: zheng
            ['郑'] = new()
            {
                PinYins = [new("zheng", CharTone.Fourth)]
            },

            // 券: quan | xuan
            ['券'] = new()
            {
                PinYins = [
                    new("quan", CharTone.Fourth),
                    new("xuan", CharTone.Fourth, cases: ["拱券", "打券"])
                ]
            },

            // 卷: juan
            ['卷'] = new()
            {
                PinYins = [new("juan", CharTone.Third)]
            },

            // 单: dan | shan | chan
            ['单'] = new()
            {
                PinYins = [
                    new("dan", CharTone.First),
                    new("shan", CharTone.Fourth, isFamilyName: true, cases: ["单县", "单劫", "单雄"]),
                    new("chan", CharTone.Second, cases: ["单于"])
                ]
            },

            // 炒: chao
            ['炒'] = new()
            {
                PinYins = [new("chao", CharTone.Third)]
            },

            // 炊: chui
            ['炊'] = new()
            {
                PinYins = [new("chui", CharTone.First)]
            },

            // 炕: kang
            ['炕'] = new()
            {
                PinYins = [new("kang", CharTone.Fourth)]
            },

            // 炎: yan
            ['炎'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 炉: lu
            ['炉'] = new()
            {
                PinYins = [new("lu", CharTone.Second)]
            },

            // 沫: mo
            ['沫'] = new()
            {
                PinYins = [new("mo", CharTone.Fourth)]
            },

            // 浅: qian
            ['浅'] = new()
            {
                PinYins = [new("qian", CharTone.Third)]
            },

            // 法: fa
            ['法'] = new()
            {
                PinYins = [new("fa", CharTone.Third)]
            },

            // 泄: xie | yi
            ['泄'] = new()
            {
                PinYins = [
                    new("xie", CharTone.Fourth),
                    new("yi", CharTone.Fourth, cases: ["泄泄"])
                ]
            },

            // 河: he
            ['河'] = new()
            {
                PinYins = [new("he", CharTone.Second)]
            },

            // 沾: zhan
            ['沾'] = new()
            {
                PinYins = [new("zhan", CharTone.First)]
            },

            // 泪: lei
            ['泪'] = new()
            {
                PinYins = [new("lei", CharTone.Fourth)]
            },

            // 油: you
            ['油'] = new()
            {
                PinYins = [new("you", CharTone.Second)]
            },

            // 泊: bo | po
            ['泊'] = new()
            {
                PinYins = [
                    new("bo", CharTone.Second),
                    new("po", CharTone.First, cases: ["湖泊", "血泊", "水泊", "泊子", "梁山泊", "罗布泊"])
                ]
            },

            // 沿: yan
            ['沿'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 泡: pao
            ['泡'] = new()
            {
                PinYins = [new("pao", CharTone.Fourth)]
            },

            // 注: zhu
            ['注'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 泻: xie
            ['泻'] = new()
            {
                PinYins = [new("xie", CharTone.Fourth)]
            },

            // 泳: yong
            ['泳'] = new()
            {
                PinYins = [new("yong", CharTone.Third)]
            },

            // 泥: ni
            ['泥'] = new()
            {
                PinYins = [new("ni", CharTone.Second)]
            },

            // 沸: fei
            ['沸'] = new()
            {
                PinYins = [new("fei", CharTone.Fourth)]
            },

            // 波: bo
            ['波'] = new()
            {
                PinYins = [new("bo", CharTone.First)]
            },

            // 泼: po
            ['泼'] = new()
            {
                PinYins = [new("po", CharTone.First)]
            },

            // 泽: ze
            ['泽'] = new()
            {
                PinYins = [new("ze", CharTone.Second)]
            },

            // 治: zhi
            ['治'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 怖: bu
            ['怖'] = new()
            {
                PinYins = [new("bu", CharTone.Fourth)]
            },

            // 性: xing
            ['性'] = new()
            {
                PinYins = [new("xing", CharTone.Fourth)]
            },

            // 怕: pa
            ['怕'] = new()
            {
                PinYins = [new("pa", CharTone.Fourth)]
            },

            // 怜: lian
            ['怜'] = new()
            {
                PinYins = [new("lian", CharTone.Second)]
            },

            // 怪: guai
            ['怪'] = new()
            {
                PinYins = [new("guai", CharTone.Fourth)]
            },

            // 学: xue
            ['学'] = new()
            {
                PinYins = [new("xue", CharTone.Second)]
            },

            // 宝: bao
            ['宝'] = new()
            {
                PinYins = [new("bao", CharTone.Third)]
            },

            // 宗: zong
            ['宗'] = new()
            {
                PinYins = [new("zong", CharTone.First)]
            },

            // 定: ding
            ['定'] = new()
            {
                PinYins = [new("ding", CharTone.Fourth)]
            },

            // 宜: yi
            ['宜'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 审: shen
            ['审'] = new()
            {
                PinYins = [new("shen", CharTone.Third)]
            },

            // 宙: zhou
            ['宙'] = new()
            {
                PinYins = [new("zhou", CharTone.Fourth)]
            },

            // 官: guan
            ['官'] = new()
            {
                PinYins = [new("guan", CharTone.First)]
            },

            // 空: kong
            ['空'] = new()
            {
                PinYins = [
                    new("kong", CharTone.First),
                    new("kong", CharTone.Fourth, cases: ["没空", "空地", "空隙", "空闲", "空余", "空格", "留空", "空缺", "偷空", "得空", "趁空", "抽空", "填空", "空额", "空当", "空白", "空儿", "空子"])
                ]
            },

            // 帘: lian
            ['帘'] = new()
            {
                PinYins = [new("lian", CharTone.Second)]
            },

            // 实: shi
            ['实'] = new()
            {
                PinYins = [new("shi", CharTone.Second)]
            },

            // 试: shi
            ['试'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 郎: lang
            ['郎'] = new()
            {
                PinYins = [new("lang", CharTone.Second)]
            },

            // 诗: shi
            ['诗'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },

            // 肩: jian
            ['肩'] = new()
            {
                PinYins = [new("jian", CharTone.First)]
            },

            // 房: fang
            ['房'] = new()
            {
                PinYins = [new("fang", CharTone.Second)]
            },

            // 诚: cheng
            ['诚'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 衬: chen
            ['衬'] = new()
            {
                PinYins = [new("chen", CharTone.Fourth)]
            },

            // 衫: shan
            ['衫'] = new()
            {
                PinYins = [new("shan", CharTone.First)]
            },

            // 视: shi
            ['视'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 话: hua
            ['话'] = new()
            {
                PinYins = [new("hua", CharTone.Fourth)]
            },

            // 诞: dan
            ['诞'] = new()
            {
                PinYins = [new("dan", CharTone.Fourth)]
            },

            // 询: xun
            ['询'] = new()
            {
                PinYins = [new("xun", CharTone.Second)]
            },

            // 该: gai
            ['该'] = new()
            {
                PinYins = [new("gai", CharTone.First)]
            },

            // 详: xiang
            ['详'] = new()
            {
                PinYins = [new("xiang", CharTone.Second)]
            },

            // 建: jian
            ['建'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 肃: su
            ['肃'] = new()
            {
                PinYins = [new("su", CharTone.Fourth)]
            },

            // 录: lu
            ['录'] = new()
            {
                PinYins = [new("lu", CharTone.Fourth)]
            },

            // 隶: li
            ['隶'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 居: ju
            ['居'] = new()
            {
                PinYins = [new("ju", CharTone.First)]
            },

            // 届: jie
            ['届'] = new()
            {
                PinYins = [new("jie", CharTone.Fourth)]
            },

            // 刷: shua
            ['刷'] = new()
            {
                PinYins = [new("shua", CharTone.First)]
            },

            // 屈: qu
            ['屈'] = new()
            {
                PinYins = [new("qu", CharTone.First)]
            },

            // 弦: xian
            ['弦'] = new()
            {
                PinYins = [new("xian", CharTone.Second)]
            },

            // 承: cheng
            ['承'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 孟: meng
            ['孟'] = new()
            {
                PinYins = [new("meng", CharTone.Fourth)]
            },

            // 孤: gu
            ['孤'] = new()
            {
                PinYins = [new("gu", CharTone.First)]
            },

            // 陕: shan
            ['陕'] = new()
            {
                PinYins = [new("shan", CharTone.Third)]
            },

            // 降: jiang | xiang
            ['降'] = new()
            {
                PinYins = [
                    new("jiang", CharTone.Fourth),
                    new("xiang", CharTone.Second, cases: ["投降", "降服"])
                ]
            },

            // 限: xian
            ['限'] = new()
            {
                PinYins = [new("xian", CharTone.Fourth)]
            },

            // 妹: mei
            ['妹'] = new()
            {
                PinYins = [new("mei", CharTone.Fourth)]
            },

            // 姑: gu
            ['姑'] = new()
            {
                PinYins = [new("gu", CharTone.First)]
            },

            // 姐: jie
            ['姐'] = new()
            {
                PinYins = [new("jie", CharTone.Third)]
            },

            // 姓: xing
            ['姓'] = new()
            {
                PinYins = [new("xing", CharTone.Fourth)]
            },

            // 始: shi
            ['始'] = new()
            {
                PinYins = [new("shi", CharTone.Third)]
            },

            // 驾: jia
            ['驾'] = new()
            {
                PinYins = [new("jia", CharTone.Fourth)]
            },

            // 参: can | cen | shen
            ['参'] = new()
            {
                PinYins = [
                    new("can", CharTone.First),
                    new("cen", CharTone.First, cases: ["参差"]),
                    new("shen", CharTone.First, cases: ["党参", "人参", "丹参", "海参"])
                ]
            },

            // 艰: jian
            ['艰'] = new()
            {
                PinYins = [new("jian", CharTone.First)]
            },

            // 线: xian
            ['线'] = new()
            {
                PinYins = [new("xian", CharTone.Fourth)]
            },

            // 练: lian
            ['练'] = new()
            {
                PinYins = [new("lian", CharTone.Fourth)]
            },

            // 组: zu
            ['组'] = new()
            {
                PinYins = [new("zu", CharTone.Third)]
            },

            // 细: xi
            ['细'] = new()
            {
                PinYins = [new("xi", CharTone.Fourth)]
            },

            // 驶: shi
            ['驶'] = new()
            {
                PinYins = [new("shi", CharTone.Third)]
            },

            // 织: zhi
            ['织'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 终: zhong
            ['终'] = new()
            {
                PinYins = [new("zhong", CharTone.First)]
            },

            // 驻: zhu
            ['驻'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 驼: tuo
            ['驼'] = new()
            {
                PinYins = [new("tuo", CharTone.Second)]
            },

            // 绍: shao
            ['绍'] = new()
            {
                PinYins = [new("shao", CharTone.Fourth)]
            },

            // 经: jing
            ['经'] = new()
            {
                PinYins = [new("jing", CharTone.First)]
            },

            // 贯: guan
            ['贯'] = new()
            {
                PinYins = [new("guan", CharTone.Fourth)]
            },

            // 奏: zou
            ['奏'] = new()
            {
                PinYins = [new("zou", CharTone.Fourth)]
            },

            // 春: chun
            ['春'] = new()
            {
                PinYins = [new("chun", CharTone.First)]
            },

            // 帮: bang
            ['帮'] = new()
            {
                PinYins = [new("bang", CharTone.First)]
            },

            // 珍: zhen
            ['珍'] = new()
            {
                PinYins = [new("zhen", CharTone.First)]
            },

            // 玻: bo
            ['玻'] = new()
            {
                PinYins = [new("bo", CharTone.First)]
            },

            // 毒: du
            ['毒'] = new()
            {
                PinYins = [new("du", CharTone.Second)]
            },

            // 型: xing
            ['型'] = new()
            {
                PinYins = [new("xing", CharTone.Second)]
            },

            // 挂: gua
            ['挂'] = new()
            {
                PinYins = [new("gua", CharTone.Fourth)]
            },

            // 封: feng
            ['封'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 持: chi
            ['持'] = new()
            {
                PinYins = [new("chi", CharTone.Second)]
            },

            // 项: xiang
            ['项'] = new()
            {
                PinYins = [new("xiang", CharTone.Fourth)]
            },

            // 垮: kua
            ['垮'] = new()
            {
                PinYins = [new("kua", CharTone.Third)]
            },

            // 挎: kua
            ['挎'] = new()
            {
                PinYins = [new("kua", CharTone.Fourth)]
            },

            // 城: cheng
            ['城'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 挠: nao
            ['挠'] = new()
            {
                PinYins = [new("nao", CharTone.Second)]
            },

            // 政: zheng
            ['政'] = new()
            {
                PinYins = [new("zheng", CharTone.Fourth)]
            },

            // 赴: fu
            ['赴'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 赵: zhao
            ['赵'] = new()
            {
                PinYins = [new("zhao", CharTone.Fourth)]
            },

            // 挡: dang
            ['挡'] = new()
            {
                PinYins = [new("dang", CharTone.Third)]
            },

            // 挺: ting
            ['挺'] = new()
            {
                PinYins = [new("ting", CharTone.Third)]
            },

            // 括: kuo
            ['括'] = new()
            {
                PinYins = [new("kuo", CharTone.Fourth)]
            },

            // 拴: shuan
            ['拴'] = new()
            {
                PinYins = [new("shuan", CharTone.First)]
            },

            // 拾: shi
            ['拾'] = new()
            {
                PinYins = [new("shi", CharTone.Second)]
            },

            // 挑: tiao
            ['挑'] = new()
            {
                PinYins = [new("tiao", CharTone.First)]
            },

            // 指: zhi
            ['指'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 垫: dian
            ['垫'] = new()
            {
                PinYins = [new("dian", CharTone.Fourth)]
            },

            // 挣: zheng
            ['挣'] = new()
            {
                PinYins = [new("zheng", CharTone.Fourth)]
            },

            // 挤: ji
            ['挤'] = new()
            {
                PinYins = [new("ji", CharTone.Third)]
            },

            // 拼: pin
            ['拼'] = new()
            {
                PinYins = [new("pin", CharTone.First)]
            },

            // 挖: wa
            ['挖'] = new()
            {
                PinYins = [new("wa", CharTone.First)]
            },

            // 按: an
            ['按'] = new()
            {
                PinYins = [new("an", CharTone.Fourth)]
            },

            // 挥: hui
            ['挥'] = new()
            {
                PinYins = [new("hui", CharTone.First)]
            },

            // 挪: nuo
            ['挪'] = new()
            {
                PinYins = [new("nuo", CharTone.Second)]
            },

            // 某: mou
            ['某'] = new()
            {
                PinYins = [new("mou", CharTone.Third)]
            },

            // 甚: shen
            ['甚'] = new()
            {
                PinYins = [new("shen", CharTone.Fourth)]
            },

            // 革: ge
            ['革'] = new()
            {
                PinYins = [new("ge", CharTone.Second)]
            },

            // 荐: jian
            ['荐'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 巷: xiang
            ['巷'] = new()
            {
                PinYins = [new("xiang", CharTone.Fourth)]
            },

            // 带: dai
            ['带'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 草: cao
            ['草'] = new()
            {
                PinYins = [new("cao", CharTone.Third)]
            },

            // 茧: jian
            ['茧'] = new()
            {
                PinYins = [new("jian", CharTone.Third)]
            },

            // 茶: cha
            ['茶'] = new()
            {
                PinYins = [new("cha", CharTone.Second)]
            },

            // 荒: huang
            ['荒'] = new()
            {
                PinYins = [new("huang", CharTone.First)]
            },

            // 茫: mang
            ['茫'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 荡: dang
            ['荡'] = new()
            {
                PinYins = [new("dang", CharTone.Fourth)]
            },

            // 荣: rong
            ['荣'] = new()
            {
                PinYins = [new("rong", CharTone.Second)]
            },

            // 故: gu
            ['故'] = new()
            {
                PinYins = [new("gu", CharTone.Fourth)]
            },

            // 胡: hu
            ['胡'] = new()
            {
                PinYins = [new("hu", CharTone.Second)]
            },

            // 南: nan | na
            ['南'] = new()
            {
                PinYins = [
                    new("nan", CharTone.Second),
                    new("na", CharTone.First, cases: ["南无"])
                ]
            },

            // 药: yao
            ['药'] = new()
            {
                PinYins = [new("yao", CharTone.Fourth)]
            },

            // 标: biao
            ['标'] = new()
            {
                PinYins = [new("biao", CharTone.First)]
            },

            // 枯: ku
            ['枯'] = new()
            {
                PinYins = [new("ku", CharTone.First)]
            },

            // 柄: bing
            ['柄'] = new()
            {
                PinYins = [new("bing", CharTone.Third)]
            },

            // 栋: dong
            ['栋'] = new()
            {
                PinYins = [new("dong", CharTone.Fourth)]
            },

            // 相: xiang
            ['相'] = new()
            {
                PinYins = [new("xiang", CharTone.First)]
            },

            // 查: cha | zha
            ['查'] = new()
            {
                PinYins = [
                    new("cha", CharTone.Second),
                    new("zha", CharTone.First, isFamilyName: true)
                ]
            },

            // 柏: bai | bo
            ['柏'] = new()
            {
                PinYins = [
                    new("bai", CharTone.Third),
                    new("bo", CharTone.Second, isFamilyName: true, cases: ["柏林", "柏拉图"]),
                    new("bo", CharTone.Fourth, cases: ["黄柏"])
                ]
            },

            // 柳: liu
            ['柳'] = new()
            {
                PinYins = [new("liu", CharTone.Third)]
            },

            // 柱: zhu
            ['柱'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 柿: shi
            ['柿'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 栏: lan
            ['栏'] = new()
            {
                PinYins = [new("lan", CharTone.Second)]
            },

            // 树: shu
            ['树'] = new()
            {
                PinYins = [new("shu", CharTone.Fourth)]
            },

            // 要: yao
            ['要'] = new()
            {
                PinYins = [new("yao", CharTone.Fourth)]
            },

            // 咸: xian
            ['咸'] = new()
            {
                PinYins = [new("xian", CharTone.Second)]
            },

            // 威: wei
            ['威'] = new()
            {
                PinYins = [new("wei", CharTone.First)]
            },

            // 歪: wai
            ['歪'] = new()
            {
                PinYins = [new("wai", CharTone.First)]
            },

            // 研: yan
            ['研'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 砖: zhuan
            ['砖'] = new()
            {
                PinYins = [new("zhuan", CharTone.First)]
            },

            // 厘: li
            ['厘'] = new()
            {
                PinYins = [new("li", CharTone.Second)]
            },

            // 厚: hou
            ['厚'] = new()
            {
                PinYins = [new("hou", CharTone.Fourth)]
            },

            // 砌: qi
            ['砌'] = new()
            {
                PinYins = [new("qi", CharTone.Fourth)]
            },

            // 砍: kan
            ['砍'] = new()
            {
                PinYins = [new("kan", CharTone.Third)]
            },

            // 面: mian
            ['面'] = new()
            {
                PinYins = [new("mian", CharTone.Fourth)]
            },

            // 耐: nai
            ['耐'] = new()
            {
                PinYins = [new("nai", CharTone.Fourth)]
            },

            // 耍: shua
            ['耍'] = new()
            {
                PinYins = [new("shua", CharTone.Third)]
            },

            // 牵: qian
            ['牵'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 残: can
            ['残'] = new()
            {
                PinYins = [new("can", CharTone.Second)]
            },

            // 殃: yang
            ['殃'] = new()
            {
                PinYins = [new("yang", CharTone.First)]
            },

            // 轻: qing
            ['轻'] = new()
            {
                PinYins = [new("qing", CharTone.First)]
            },

            // 鸦: ya
            ['鸦'] = new()
            {
                PinYins = [new("ya", CharTone.First)]
            },

            // 皆: jie
            ['皆'] = new()
            {
                PinYins = [new("jie", CharTone.First)]
            },

            // 背: bei
            ['背'] = new()
            {
                PinYins = [
                    new("bei", CharTone.Fourth),
                    new("bei", CharTone.First, cases: ["背包", "背债", "背带", "背筐", "背负", "背饥荒", "背黑锅"])
                ]
            },

            // 战: zhan
            ['战'] = new()
            {
                PinYins = [new("zhan", CharTone.Fourth)]
            },

            // 点: dian
            ['点'] = new()
            {
                PinYins = [new("dian", CharTone.Third)]
            },

            // 临: lin
            ['临'] = new()
            {
                PinYins = [new("lin", CharTone.Second)]
            },

            // 览: lan
            ['览'] = new()
            {
                PinYins = [new("lan", CharTone.Third)]
            },

            // 竖: shu
            ['竖'] = new()
            {
                PinYins = [new("shu", CharTone.Fourth)]
            },

            // 省: sheng | xing
            ['省'] = new()
            {
                PinYins = [
                    new("sheng", CharTone.Third),
                    new("xing", CharTone.Third, cases: ["反省", "省察", "省悟", "省亲", "省视", "发人深省", "三省吾身"])
                ]
            },

            // 削: xiao | xue
            ['削'] = new()
            {
                PinYins = [
                    new("xiao", CharTone.First),
                    new("xue", CharTone.First, cases: ["削减", "削弱", "剥削", "削职", "削铁如泥", "削足适履"])
                ]
            },

            // 尝: chang
            ['尝'] = new()
            {
                PinYins = [new("chang", CharTone.Second)]
            },

            // 是: shi
            ['是'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 盼: pan
            ['盼'] = new()
            {
                PinYins = [new("pan", CharTone.Fourth)]
            },

            // 眨: zha
            ['眨'] = new()
            {
                PinYins = [new("zha", CharTone.Third)]
            },

            // 哄: hong
            ['哄'] = new()
            {
                PinYins = [
                    new("hong", CharTone.Third),
                    new("hong", CharTone.First, cases: ["哄哄", "哄闹", "哄然", "哄笑", "哄抢", "哄动", "哄传", "哄抬", "喧哄", "撺哄", "哄堂", "大哄大嗡", "撺哄鸟乱"]),
                    new("hong", CharTone.Fourth, cases: ["一哄", "哄场", "打哄", "搅哄"])
                ]
            },

            // 显: xian
            ['显'] = new()
            {
                PinYins = [new("xian", CharTone.Third)]
            },

            // 哑: ya
            ['哑'] = new()
            {
                PinYins = [new("ya", CharTone.Third)]
            },

            // 冒: mao
            ['冒'] = new()
            {
                PinYins = [new("mao", CharTone.Fourth)]
            },

            // 映: ying
            ['映'] = new()
            {
                PinYins = [new("ying", CharTone.Fourth)]
            },

            // 星: xing
            ['星'] = new()
            {
                PinYins = [new("xing", CharTone.First)]
            },

            // 昨: zuo
            ['昨'] = new()
            {
                PinYins = [new("zuo", CharTone.Second)]
            },

            // 畏: wei
            ['畏'] = new()
            {
                PinYins = [new("wei", CharTone.Fourth)]
            },

            // 趴: pa
            ['趴'] = new()
            {
                PinYins = [new("pa", CharTone.First)]
            },

            // 胃: wei
            ['胃'] = new()
            {
                PinYins = [new("wei", CharTone.Fourth)]
            },

            // 贵: gui
            ['贵'] = new()
            {
                PinYins = [new("gui", CharTone.Fourth)]
            },

            // 界: jie
            ['界'] = new()
            {
                PinYins = [new("jie", CharTone.Fourth)]
            },

            // 虹: hong
            ['虹'] = new()
            {
                PinYins = [new("hong", CharTone.Second)]
            },

            // 虾: xia
            ['虾'] = new()
            {
                PinYins = [new("xia", CharTone.First)]
            },

            // 蚁: yi
            ['蚁'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 思: si
            ['思'] = new()
            {
                PinYins = [new("si", CharTone.First)]
            },

            // 蚂: ma
            ['蚂'] = new()
            {
                PinYins = [new("ma", CharTone.Third)]
            },

            // 虽: sui
            ['虽'] = new()
            {
                PinYins = [new("sui", CharTone.First)]
            },

            // 品: pin
            ['品'] = new()
            {
                PinYins = [new("pin", CharTone.Third)]
            },

            // 咽: yan | ye
            ['咽'] = new()
            {
                PinYins = [
                    new("yan", CharTone.Fourth),
                    new("yan", CharTone.First, cases: ["咽泣", "咽喉", "呦咽", "咽唾"]),
                    new("ye", CharTone.Fourth, cases: ["哽咽", "呜咽", "幽咽"])
                ]
            },

            // 骂: ma
            ['骂'] = new()
            {
                PinYins = [new("ma", CharTone.Fourth)]
            },

            // 哗: hua
            ['哗'] = new()
            {
                PinYins = [new("hua", CharTone.First)]
            },

            // 咱: zan | za
            ['咱'] = new()
            {
                PinYins = [
                    new("zan", CharTone.Second),
                    new("za", CharTone.Second, cases: ["咱家"])
                ]
            },

            // 响: xiang
            ['响'] = new()
            {
                PinYins = [new("xiang", CharTone.Third)]
            },

            // 哈: ha
            ['哈'] = new()
            {
                PinYins = [new("ha", CharTone.First)]
            },

            // 咬: yao
            ['咬'] = new()
            {
                PinYins = [new("yao", CharTone.Third)]
            },

            // 咳: ke
            ['咳'] = new()
            {
                PinYins = [new("ke", CharTone.Second)]
            },

            // 哪: na | ne
            ['哪'] = new()
            {
                PinYins = [
                    new("na", CharTone.Third),
                    new("ne", CharTone.Second, cases: ["哪吒"])
                ]
            },

            // 炭: tan
            ['炭'] = new()
            {
                PinYins = [new("tan", CharTone.Fourth)]
            },

            // 峡: xia
            ['峡'] = new()
            {
                PinYins = [new("xia", CharTone.Second)]
            },

            // 罚: fa
            ['罚'] = new()
            {
                PinYins = [new("fa", CharTone.Second)]
            },

            // 贱: jian
            ['贱'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 贴: tie
            ['贴'] = new()
            {
                PinYins = [new("tie", CharTone.First)]
            },

            // 骨: gu
            ['骨'] = new()
            {
                PinYins = [new("gu", CharTone.Third)]
            },

            // 钞: chao
            ['钞'] = new()
            {
                PinYins = [new("chao", CharTone.First)]
            },

            // 钟: zhong
            ['钟'] = new()
            {
                PinYins = [new("zhong", CharTone.First)]
            },

            // 钢: gang
            ['钢'] = new()
            {
                PinYins = [new("gang", CharTone.First)]
            },

            // 钥: yao
            ['钥'] = new()
            {
                PinYins = [new("yao", CharTone.Fourth)]
            },

            // 钩: gou
            ['钩'] = new()
            {
                PinYins = [new("gou", CharTone.First)]
            },

            // 卸: xie
            ['卸'] = new()
            {
                PinYins = [new("xie", CharTone.Fourth)]
            },

            // 缸: gang
            ['缸'] = new()
            {
                PinYins = [new("gang", CharTone.First)]
            },

            // 拜: bai
            ['拜'] = new()
            {
                PinYins = [new("bai", CharTone.Fourth)]
            },

            // 看: kan
            ['看'] = new()
            {
                PinYins = [
                    new("kan", CharTone.Fourth),
                    new("kan", CharTone.First, cases: ["看门", "看守", "看押"])
                ]
            },

            // 矩: ju
            ['矩'] = new()
            {
                PinYins = [new("ju", CharTone.Third)]
            },

            // 怎: zen
            ['怎'] = new()
            {
                PinYins = [new("zen", CharTone.Third)]
            },

            // 牲: sheng
            ['牲'] = new()
            {
                PinYins = [new("sheng", CharTone.First)]
            },

            // 选: xuan
            ['选'] = new()
            {
                PinYins = [new("xuan", CharTone.Third)]
            },

            // 适: shi
            ['适'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 秒: miao
            ['秒'] = new()
            {
                PinYins = [new("miao", CharTone.Third)]
            },

            // 香: xiang
            ['香'] = new()
            {
                PinYins = [new("xiang", CharTone.First)]
            },

            // 种: zhong
            ['种'] = new()
            {
                PinYins = [new("zhong", CharTone.Third)]
            },

            // 秋: qiu
            ['秋'] = new()
            {
                PinYins = [new("qiu", CharTone.First)]
            },

            // 科: ke
            ['科'] = new()
            {
                PinYins = [new("ke", CharTone.First)]
            },

            // 重: zhong | chong
            ['重'] = new()
            {
                PinYins = [
                    new("zhong", CharTone.Fourth),
                    new("chong", CharTone.Second, cases: ["重庆", "重复", "重新", "重返", "重逢", "重播", "重编", "重重", "重读", "重峦叠嶂"])
                ]
            },

            // 复: fu
            ['复'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 竿: gan
            ['竿'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 段: duan
            ['段'] = new()
            {
                PinYins = [new("duan", CharTone.Fourth)]
            },

            // 便: bian | pian
            ['便'] = new()
            {
                PinYins = [
                    new("bian", CharTone.Fourth),
                    new("pian", CharTone.Second, cases: ["便便", "便佞", "便旋", "便宜"])
                ]
            },

            // 俩: liang | lia
            ['俩'] = new()
            {
                PinYins = [
                    new("liang", CharTone.Third),
                    new("lia", CharTone.Third, cases: ["咱俩", "你俩", "你们俩", "俩人"])
                ]
            },

            // 贷: dai
            ['贷'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 顺: shun
            ['顺'] = new()
            {
                PinYins = [new("shun", CharTone.Fourth)]
            },

            // 修: xiu
            ['修'] = new()
            {
                PinYins = [new("xiu", CharTone.First)]
            },

            // 保: bao
            ['保'] = new()
            {
                PinYins = [new("bao", CharTone.Third)]
            },

            // 促: cu
            ['促'] = new()
            {
                PinYins = [new("cu", CharTone.Fourth)]
            },

            // 侮: wu
            ['侮'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 俭: jian
            ['俭'] = new()
            {
                PinYins = [new("jian", CharTone.Third)]
            },

            // 俗: su
            ['俗'] = new()
            {
                PinYins = [new("su", CharTone.Second)]
            },

            // 俘: fu
            ['俘'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 信: xin | shen
            ['信'] = new()
            {
                PinYins = [
                    new("xin", CharTone.Fourth),
                    new("shen", CharTone.First, isFamilyName: true)
                ]
            },

            // 皇: huang
            ['皇'] = new()
            {
                PinYins = [new("huang", CharTone.Second)]
            },

            // 泉: quan
            ['泉'] = new()
            {
                PinYins = [new("quan", CharTone.Second)]
            },

            // 鬼: gui
            ['鬼'] = new()
            {
                PinYins = [new("gui", CharTone.Third)]
            },

            // 侵: qin
            ['侵'] = new()
            {
                PinYins = [new("qin", CharTone.First)]
            },

            // 追: zhui
            ['追'] = new()
            {
                PinYins = [new("zhui", CharTone.First)]
            },

            // 俊: jun
            ['俊'] = new()
            {
                PinYins = [new("jun", CharTone.Fourth)]
            },

            // 盾: dun
            ['盾'] = new()
            {
                PinYins = [new("dun", CharTone.Fourth)]
            },

            // 待: dai
            ['待'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 律: lv
            ['律'] = new()
            {
                PinYins = [new("lv", CharTone.Fourth)]
            },

            // 很: hen
            ['很'] = new()
            {
                PinYins = [new("hen", CharTone.Third)]
            },

            // 须: xu
            ['须'] = new()
            {
                PinYins = [new("xu", CharTone.First)]
            },

            // 叙: xu
            ['叙'] = new()
            {
                PinYins = [new("xu", CharTone.Fourth)]
            },

            // 剑: jian
            ['剑'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 逃: tao
            ['逃'] = new()
            {
                PinYins = [new("tao", CharTone.Second)]
            },

            // 食: shi | si
            ['食'] = new()
            {
                PinYins = [
                    new("shi", CharTone.Second),
                    new("si", CharTone.Fourth, cases: ["食母"])
                ]
            },

            // 盆: pen
            ['盆'] = new()
            {
                PinYins = [new("pen", CharTone.Second)]
            },

            // 胆: dan
            ['胆'] = new()
            {
                PinYins = [new("dan", CharTone.Third)]
            },

            // 胜: sheng
            ['胜'] = new()
            {
                PinYins = [new("sheng", CharTone.Fourth)]
            },

            // 胞: bao
            ['胞'] = new()
            {
                PinYins = [new("bao", CharTone.First)]
            },

            // 胖: pang | pan
            ['胖'] = new()
            {
                PinYins = [
                    new("pang", CharTone.Fourth),
                    new("pan", CharTone.Second, cases: ["心宽体胖"])
                ]
            },

            // 脉: mai | mo
            ['脉'] = new()
            {
                PinYins = [
                    new("mai", CharTone.Fourth),
                    new("mo", CharTone.Fourth, cases: ["脉脉"])
                ]
            },

            // 勉: mian
            ['勉'] = new()
            {
                PinYins = [new("mian", CharTone.Third)]
            },

            // 狭: xia
            ['狭'] = new()
            {
                PinYins = [new("xia", CharTone.Second)]
            },

            // 狮: shi
            ['狮'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },

            // 独: du
            ['独'] = new()
            {
                PinYins = [new("du", CharTone.Second)]
            },

            // 狡: jiao
            ['狡'] = new()
            {
                PinYins = [new("jiao", CharTone.Third)]
            },

            // 狱: yu
            ['狱'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 狠: hen
            ['狠'] = new()
            {
                PinYins = [new("hen", CharTone.Third)]
            },

            // 贸: mao
            ['贸'] = new()
            {
                PinYins = [new("mao", CharTone.Fourth)]
            },

            // 怨: yuan
            ['怨'] = new()
            {
                PinYins = [new("yuan", CharTone.Fourth)]
            },

            // 急: ji
            ['急'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 饶: rao
            ['饶'] = new()
            {
                PinYins = [new("rao", CharTone.Second)]
            },

            // 蚀: shi
            ['蚀'] = new()
            {
                PinYins = [new("shi", CharTone.Second)]
            },

            // 饺: jiao
            ['饺'] = new()
            {
                PinYins = [new("jiao", CharTone.Third)]
            },

            // 饼: bing
            ['饼'] = new()
            {
                PinYins = [new("bing", CharTone.Third)]
            },

            // 弯: wan
            ['弯'] = new()
            {
                PinYins = [new("wan", CharTone.First)]
            },

            // 将: jiang
            ['将'] = new()
            {
                PinYins = [new("jiang", CharTone.First)]
            },

            // 奖: jiang
            ['奖'] = new()
            {
                PinYins = [new("jiang", CharTone.Third)]
            },

            // 哀: ai
            ['哀'] = new()
            {
                PinYins = [new("ai", CharTone.First)]
            },

            // 亭: ting
            ['亭'] = new()
            {
                PinYins = [new("ting", CharTone.Second)]
            },

            // 亮: liang
            ['亮'] = new()
            {
                PinYins = [new("liang", CharTone.Fourth)]
            },

            // 度: du | duo
            ['度'] = new()
            {
                PinYins = [
                    new("du", CharTone.Fourth),
                    new("duo", CharTone.Fourth, cases: ["揣度", "忖度", "审时度势", "度德量力"])
                ]
            },

            // 迹: ji
            ['迹'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 庭: ting
            ['庭'] = new()
            {
                PinYins = [new("ting", CharTone.Second)]
            },

            // 疮: chuang
            ['疮'] = new()
            {
                PinYins = [new("chuang", CharTone.First)]
            },

            // 疯: feng
            ['疯'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 疫: yi
            ['疫'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 疤: ba
            ['疤'] = new()
            {
                PinYins = [new("ba", CharTone.First)]
            },

            // 姿: zi
            ['姿'] = new()
            {
                PinYins = [new("zi", CharTone.First)]
            },

            // 亲: qin | qing
            ['亲'] = new()
            {
                PinYins = [
                    new("qin", CharTone.First),
                    new("qing", CharTone.Fourth, cases: ["亲家"])
                ]
            },

            // 音: yin
            ['音'] = new()
            {
                PinYins = [new("yin", CharTone.First)]
            },

            // 帝: di
            ['帝'] = new()
            {
                PinYins = [new("di", CharTone.Fourth)]
            },

            // 施: shi
            ['施'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },

            // 闻: wen
            ['闻'] = new()
            {
                PinYins = [new("wen", CharTone.Second)]
            },

            // 阀: fa
            ['阀'] = new()
            {
                PinYins = [new("fa", CharTone.Second)]
            },

            // 阁: ge
            ['阁'] = new()
            {
                PinYins = [new("ge", CharTone.Second)]
            },

            // 差: cha | chai | ci
            ['差'] = new()
            {
                PinYins = [
                    new("cha", CharTone.Fourth),
                    new("cha", CharTone.First, cases: ["差别", "差异", "差错", "差价", "差强人意", "差之毫厘"]),
                    new("chai", CharTone.First, cases: ["差遣", "差人", "差役", "差使", "出差"]),
                    new("ci", CharTone.First, cases: ["参差"])
                ]
            },

            // 养: yang
            ['养'] = new()
            {
                PinYins = [new("yang", CharTone.Third)]
            },

            // 美: mei
            ['美'] = new()
            {
                PinYins = [new("mei", CharTone.Third)]
            },

            // 姜: jiang
            ['姜'] = new()
            {
                PinYins = [new("jiang", CharTone.First)]
            },

            // 叛: pan
            ['叛'] = new()
            {
                PinYins = [new("pan", CharTone.Fourth)]
            },

            // 送: song
            ['送'] = new()
            {
                PinYins = [new("song", CharTone.Fourth)]
            },

            // 类: lei
            ['类'] = new()
            {
                PinYins = [new("lei", CharTone.Fourth)]
            },

            // 迷: mi
            ['迷'] = new()
            {
                PinYins = [new("mi", CharTone.Second)]
            },

            // 前: qian
            ['前'] = new()
            {
                PinYins = [new("qian", CharTone.Second)]
            },

            // 首: shou
            ['首'] = new()
            {
                PinYins = [new("shou", CharTone.Third)]
            },

            // 逆: ni
            ['逆'] = new()
            {
                PinYins = [new("ni", CharTone.Fourth)]
            },

            // 总: zong
            ['总'] = new()
            {
                PinYins = [new("zong", CharTone.Third)]
            },

            // 炼: lian
            ['炼'] = new()
            {
                PinYins = [new("lian", CharTone.Fourth)]
            },

            // 炸: zha
            ['炸'] = new()
            {
                PinYins = [new("zha", CharTone.Fourth)]
            },

            // 炮: pao | bao | pao
            ['炮'] = new()
            {
                PinYins = [
                    new("pao", CharTone.Fourth),
                    new("pao", CharTone.Second, cases: ["炮烙", "炮制", "炮姜"])
                ]
            },

            // 烂: lan
            ['烂'] = new()
            {
                PinYins = [new("lan", CharTone.Fourth)]
            },

            // 剃: ti
            ['剃'] = new()
            {
                PinYins = [new("ti", CharTone.Fourth)]
            },

            // 洁: jie
            ['洁'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 洪: hong
            ['洪'] = new()
            {
                PinYins = [new("hong", CharTone.Second)]
            },

            // 洒: sa
            ['洒'] = new()
            {
                PinYins = [new("sa", CharTone.Third)]
            },

            // 浇: jiao
            ['浇'] = new()
            {
                PinYins = [new("jiao", CharTone.First)]
            },

            // 浊: zhuo
            ['浊'] = new()
            {
                PinYins = [new("zhuo", CharTone.Second)]
            },

            // 洞: dong
            ['洞'] = new()
            {
                PinYins = [new("dong", CharTone.Fourth)]
            },

            // 测: ce
            ['测'] = new()
            {
                PinYins = [new("ce", CharTone.Fourth)]
            },

            // 洗: xi | xian
            ['洗'] = new()
            {
                PinYins = [
                    new("xi", CharTone.Third),
                    new("xian", CharTone.Third, isFamilyName: true)
                ]
            },

            // 活: huo
            ['活'] = new()
            {
                PinYins = [new("huo", CharTone.Second)]
            },

            // 派: pai
            ['派'] = new()
            {
                PinYins = [new("pai", CharTone.Fourth)]
            },

            // 洽: qia
            ['洽'] = new()
            {
                PinYins = [new("qia", CharTone.Fourth)]
            },

            // 染: ran
            ['染'] = new()
            {
                PinYins = [new("ran", CharTone.Third)]
            },

            // 济: ji
            ['济'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 洋: yang
            ['洋'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 洲: zhou
            ['洲'] = new()
            {
                PinYins = [new("zhou", CharTone.First)]
            },

            // 浑: hun
            ['浑'] = new()
            {
                PinYins = [new("hun", CharTone.Second)]
            },

            // 浓: nong
            ['浓'] = new()
            {
                PinYins = [new("nong", CharTone.Second)]
            },

            // 津: jin
            ['津'] = new()
            {
                PinYins = [new("jin", CharTone.First)]
            },

            // 恒: heng
            ['恒'] = new()
            {
                PinYins = [new("heng", CharTone.Second)]
            },

            // 恢: hui
            ['恢'] = new()
            {
                PinYins = [new("hui", CharTone.First)]
            },

            // 恰: qia
            ['恰'] = new()
            {
                PinYins = [new("qia", CharTone.Fourth)]
            },

            // 恼: nao
            ['恼'] = new()
            {
                PinYins = [new("nao", CharTone.Third)]
            },

            // 恨: hen
            ['恨'] = new()
            {
                PinYins = [new("hen", CharTone.Fourth)]
            },

            // 举: ju
            ['举'] = new()
            {
                PinYins = [new("ju", CharTone.Third)]
            },

            // 觉: jue | jiao
            ['觉'] = new()
            {
                PinYins = [
                    new("jue", CharTone.Second),
                    new("jiao", CharTone.Fourth, cases: ["午觉", "睡觉", "困觉", "晌觉", "午觉", "懒觉", "咪觉", "一觉"])
                ]
            },

            // 宣: xuan
            ['宣'] = new()
            {
                PinYins = [new("xuan", CharTone.First)]
            },

            // 室: shi
            ['室'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 宫: gong
            ['宫'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 宪: xian
            ['宪'] = new()
            {
                PinYins = [new("xian", CharTone.Fourth)]
            },

            // 突: tu
            ['突'] = new()
            {
                PinYins = [new("tu", CharTone.First)]
            },

            // 穿: chuan
            ['穿'] = new()
            {
                PinYins = [new("chuan", CharTone.First)]
            },

            // 窃: qie
            ['窃'] = new()
            {
                PinYins = [new("qie", CharTone.Fourth)]
            },

            // 客: ke
            ['客'] = new()
            {
                PinYins = [new("ke", CharTone.Fourth)]
            },

            // 冠: guan
            ['冠'] = new()
            {
                PinYins = [
                    new("guan", CharTone.First),
                    new("guan", CharTone.Fourth, cases: ["弱冠", "冠军", "冠士", "冠序", "冠首", "冠岁", "冠群", "冠词", "冲冠", "气冠", "弱冠", "冠礼", "未冠", "衣冠", "冠族", "冠篇", "冠带", "黄冠", "勇冠", "连冠", "超今冠古", "冠绝一时"])
                ]
            },

            // 语: yu
            ['语'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 扁: bian | pian
            ['扁'] = new()
            {
                PinYins = [
                    new("bian", CharTone.Third),
                    new("pian", CharTone.First, cases: ["扁舟", "扁乘", "扁枯"])
                ]
            },

            // 袄: ao
            ['袄'] = new()
            {
                PinYins = [new("ao", CharTone.Third)]
            },

            // 祖: zu
            ['祖'] = new()
            {
                PinYins = [new("zu", CharTone.Third)]
            },

            // 神: shen
            ['神'] = new()
            {
                PinYins = [new("shen", CharTone.Second)]
            },

            // 祝: zhu
            ['祝'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 误: wu
            ['误'] = new()
            {
                PinYins = [new("wu", CharTone.Fourth)]
            },

            // 诱: you
            ['诱'] = new()
            {
                PinYins = [new("you", CharTone.Fourth)]
            },

            // 说: shuo | shui
            ['说'] = new()
            {
                PinYins = [
                    new("shuo", CharTone.First),
                    new("shui", CharTone.Fourth, cases: ["游说"])
                ]
            },

            // 诵: song
            ['诵'] = new()
            {
                PinYins = [new("song", CharTone.Fourth)]
            },

            // 垦: ken
            ['垦'] = new()
            {
                PinYins = [new("ken", CharTone.Third)]
            },

            // 退: tui
            ['退'] = new()
            {
                PinYins = [new("tui", CharTone.Fourth)]
            },

            // 既: ji
            ['既'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 屋: wu
            ['屋'] = new()
            {
                PinYins = [new("wu", CharTone.First)]
            },

            // 昼: zhou
            ['昼'] = new()
            {
                PinYins = [new("zhou", CharTone.Fourth)]
            },

            // 费: fei
            ['费'] = new()
            {
                PinYins = [new("fei", CharTone.Fourth)]
            },

            // 陡: dou
            ['陡'] = new()
            {
                PinYins = [new("dou", CharTone.Third)]
            },

            // 眉: mei
            ['眉'] = new()
            {
                PinYins = [new("mei", CharTone.Second)]
            },

            // 孩: hai
            ['孩'] = new()
            {
                PinYins = [new("hai", CharTone.Second)]
            },

            // 除: chu
            ['除'] = new()
            {
                PinYins = [new("chu", CharTone.Second)]
            },

            // 险: xian
            ['险'] = new()
            {
                PinYins = [new("xian", CharTone.Third)]
            },

            // 院: yuan
            ['院'] = new()
            {
                PinYins = [new("yuan", CharTone.Fourth)]
            },

            // 娃: wa
            ['娃'] = new()
            {
                PinYins = [new("wa", CharTone.Second)]
            },

            // 姥: mu | lao
            ['姥'] = new()
            {
                PinYins = [
                    new("lao", CharTone.Third),
                    new("mu", CharTone.Third, cases: ["公姥", "太姥山"])
                ]
            },

            // 姨: yi
            ['姨'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 姻: yin
            ['姻'] = new()
            {
                PinYins = [new("yin", CharTone.First)]
            },

            // 娇: jiao
            ['娇'] = new()
            {
                PinYins = [new("jiao", CharTone.First)]
            },

            // 怒: nu
            ['怒'] = new()
            {
                PinYins = [new("nu", CharTone.Fourth)]
            },

            // 架: jia
            ['架'] = new()
            {
                PinYins = [new("jia", CharTone.Fourth)]
            },

            // 贺: he
            ['贺'] = new()
            {
                PinYins = [new("he", CharTone.Fourth)]
            },

            // 盈: ying
            ['盈'] = new()
            {
                PinYins = [new("ying", CharTone.Second)]
            },

            // 勇: yong
            ['勇'] = new()
            {
                PinYins = [new("yong", CharTone.Third)]
            },

            // 怠: dai
            ['怠'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 柔: rou
            ['柔'] = new()
            {
                PinYins = [new("rou", CharTone.Second)]
            },

            // 垒: lei
            ['垒'] = new()
            {
                PinYins = [new("lei", CharTone.Third)]
            },

            // 绑: bang
            ['绑'] = new()
            {
                PinYins = [new("bang", CharTone.Third)]
            },

            // 绒: rong
            ['绒'] = new()
            {
                PinYins = [new("rong", CharTone.Second)]
            },

            // 结: jie
            ['结'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 绕: rao
            ['绕'] = new()
            {
                PinYins = [new("rao", CharTone.Fourth)]
            },

            // 骄: jiao
            ['骄'] = new()
            {
                PinYins = [new("jiao", CharTone.First)]
            },

            // 绘: hui
            ['绘'] = new()
            {
                PinYins = [new("hui", CharTone.Fourth)]
            },

            // 给: gei | ji
            ['给'] = new()
            {
                PinYins = [
                    new("gei", CharTone.Third),
                    new("ji", CharTone.Third, cases: ["供给", "补给", "给养", "家给人足", "自给自足"])
                ]
            },

            // 络: luo | lao
            ['络'] = new()
            {
                PinYins = [
                    new("luo", CharTone.Fourth),
                    new("lao", CharTone.Fourth, cases: ["络子"])
                ]
            },

            // 骆: luo
            ['骆'] = new()
            {
                PinYins = [new("luo", CharTone.Fourth)]
            },

            // 绝: jue
            ['绝'] = new()
            {
                PinYins = [new("jue", CharTone.Second)]
            },

            // 绞: jiao
            ['绞'] = new()
            {
                PinYins = [new("jiao", CharTone.Third)]
            },

            // 统: tong
            ['统'] = new()
            {
                PinYins = [new("tong", CharTone.Third)]
            },

            // 耕: geng
            ['耕'] = new()
            {
                PinYins = [new("geng", CharTone.First)]
            },

            // 耗: hao
            ['耗'] = new()
            {
                PinYins = [new("hao", CharTone.Fourth)]
            },

            // 艳: yan
            ['艳'] = new()
            {
                PinYins = [new("yan", CharTone.Fourth)]
            },

            // 泰: tai
            ['泰'] = new()
            {
                PinYins = [new("tai", CharTone.Fourth)]
            },

            // 珠: zhu
            ['珠'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 班: ban
            ['班'] = new()
            {
                PinYins = [new("ban", CharTone.First)]
            },

            // 素: su
            ['素'] = new()
            {
                PinYins = [new("su", CharTone.Fourth)]
            },

            // 蚕: can
            ['蚕'] = new()
            {
                PinYins = [new("can", CharTone.Second)]
            },

            // 顽: wan
            ['顽'] = new()
            {
                PinYins = [new("wan", CharTone.Second)]
            },

            // 盏: zhan
            ['盏'] = new()
            {
                PinYins = [new("zhan", CharTone.Third)]
            },

            // 匪: fei
            ['匪'] = new()
            {
                PinYins = [new("fei", CharTone.Third)]
            },

            // 捞: lao
            ['捞'] = new()
            {
                PinYins = [new("lao", CharTone.First)]
            },

            // 栽: zai
            ['栽'] = new()
            {
                PinYins = [new("zai", CharTone.First)]
            },

            // 捕: bu
            ['捕'] = new()
            {
                PinYins = [new("bu", CharTone.Third)]
            },

            // 振: zhen
            ['振'] = new()
            {
                PinYins = [new("zhen", CharTone.Fourth)]
            },

            // 载: zai
            ['载'] = new()
            {
                PinYins = [
                    new("zai", CharTone.Third),
                    new("zai", CharTone.Fourth, cases: ["载运", "载重", "载荷", "载波", "载物", "载体", "载人", "载道", "载途", "载舟", "载路", "载客", "载歌", "载舞", "载誉", "装载", "加载", "运载", "负载", "满载", "承载", "过载", "下载", "卸载", "载畜量", "怨声载道", "载欢载笑"])
                ]
            },

            // 赶: gan
            ['赶'] = new()
            {
                PinYins = [new("gan", CharTone.Third)]
            },

            // 起: qi
            ['起'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 盐: yan
            ['盐'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 捎: shao
            ['捎'] = new()
            {
                PinYins = [new("shao", CharTone.First)]
            },

            // 捏: nie
            ['捏'] = new()
            {
                PinYins = [new("nie", CharTone.First)]
            },

            // 埋: mai | man
            ['埋'] = new()
            {
                PinYins = [
                    new("mai", CharTone.Second),
                    new("man", CharTone.Second, cases: ["埋怨", "埋天怨地", "埋三怨四"])
                ]
            },

            // 捉: zhuo
            ['捉'] = new()
            {
                PinYins = [new("zhuo", CharTone.First)]
            },

            // 捆: kun
            ['捆'] = new()
            {
                PinYins = [new("kun", CharTone.Third)]
            },

            // 捐: juan
            ['捐'] = new()
            {
                PinYins = [new("juan", CharTone.First)]
            },

            // 损: sun
            ['损'] = new()
            {
                PinYins = [new("sun", CharTone.Third)]
            },

            // 都: du | dou
            ['都'] = new()
            {
                PinYins = [
                    new("du", CharTone.First),
                    new("dou", CharTone.First, cases: ["都来", "全都", "都吏", "野都"])
                ]
            },

            // 哲: zhe
            ['哲'] = new()
            {
                PinYins = [new("zhe", CharTone.Second)]
            },

            // 逝: shi
            ['逝'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 捡: jian
            ['捡'] = new()
            {
                PinYins = [new("jian", CharTone.Third)]
            },

            // 换: huan
            ['换'] = new()
            {
                PinYins = [new("huan", CharTone.Fourth)]
            },

            // 挽: wan
            ['挽'] = new()
            {
                PinYins = [new("wan", CharTone.Third)]
            },

            // 热: re
            ['热'] = new()
            {
                PinYins = [new("re", CharTone.Fourth)]
            },

            // 恐: kong
            ['恐'] = new()
            {
                PinYins = [new("kong", CharTone.Third)]
            },

            // 壶: hu
            ['壶'] = new()
            {
                PinYins = [new("hu", CharTone.Second)]
            },

            // 挨: ai
            ['挨'] = new()
            {
                PinYins = [new("ai", CharTone.First)]
            },

            // 耻: chi
            ['耻'] = new()
            {
                PinYins = [new("chi", CharTone.Third)]
            },

            // 耽: dan
            ['耽'] = new()
            {
                PinYins = [new("dan", CharTone.First)]
            },

            // 恭: gong
            ['恭'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 莲: lian
            ['莲'] = new()
            {
                PinYins = [new("lian", CharTone.Second)]
            },

            // 莫: mo
            ['莫'] = new()
            {
                PinYins = [new("mo", CharTone.Fourth)]
            },

            // 荷: he
            ['荷'] = new()
            {
                PinYins = [
                    new("he", CharTone.Second),
                    new("he", CharTone.Fourth, cases: ["负荷", "电荷", "载荷", "荷载", "感荷"])
                ]
            },

            // 获: huo
            ['获'] = new()
            {
                PinYins = [new("huo", CharTone.Fourth)]
            },

            // 晋: jin
            ['晋'] = new()
            {
                PinYins = [new("jin", CharTone.Fourth)]
            },

            // 恶: e | wu
            ['恶'] = new()
            {
                PinYins = [
                    new("e", CharTone.Fourth),
                    new("e", CharTone.Third, cases: ["恶心"]),
                    new("wu", CharTone.Fourth, cases: ["厌恶", "可恶", "嫌恶", "憎恶", "好恶", "交恶", "羞恶", "痛恶", "恶欲", "恶杀", "恶寒", "同恶", "恶恶", "恨恶", "众恶", "疾恶", "四恶趣", "深恶"])
                ]
            },

            // 真: zhen
            ['真'] = new()
            {
                PinYins = [new("zhen", CharTone.First)]
            },

            // 框: kuang
            ['框'] = new()
            {
                PinYins = [new("kuang", CharTone.First)]
            },

            // 桂: gui
            ['桂'] = new()
            {
                PinYins = [new("gui", CharTone.Fourth)]
            },

            // 档: dang
            ['档'] = new()
            {
                PinYins = [new("dang", CharTone.Fourth)]
            },

            // 桐: tong
            ['桐'] = new()
            {
                PinYins = [new("tong", CharTone.Second)]
            },

            // 株: zhu
            ['株'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 桥: qiao
            ['桥'] = new()
            {
                PinYins = [new("qiao", CharTone.Second)]
            },

            // 桃: tao
            ['桃'] = new()
            {
                PinYins = [new("tao", CharTone.Second)]
            },

            // 格: ge
            ['格'] = new()
            {
                PinYins = [new("ge", CharTone.Second)]
            },

            // 校: xiao | jiao
            ['校'] = new()
            {
                PinYins = [
                    new("xiao", CharTone.Fourth),
                    new("jiao", CharTone.Fourth, cases: ["校对", "校场", "校勘", "校订"])
                ]
            },

            // 核: he | hu
            ['核'] = new()
            {
                PinYins = [
                    new("he", CharTone.Second),
                    new("hu", CharTone.Second, cases: ["杏核", "桃核", "梨核", "煤核"])
                ]
            },

            // 样: yang
            ['样'] = new()
            {
                PinYins = [new("yang", CharTone.Fourth)]
            },

            // 根: gen
            ['根'] = new()
            {
                PinYins = [new("gen", CharTone.First)]
            },

            // 索: suo
            ['索'] = new()
            {
                PinYins = [new("suo", CharTone.Third)]
            },

            // 哥: ge
            ['哥'] = new()
            {
                PinYins = [new("ge", CharTone.First)]
            },

            // 速: su
            ['速'] = new()
            {
                PinYins = [new("su", CharTone.Fourth)]
            },

            // 逗: dou
            ['逗'] = new()
            {
                PinYins = [new("dou", CharTone.Fourth)]
            },

            // 栗: li
            ['栗'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 配: pei
            ['配'] = new()
            {
                PinYins = [new("pei", CharTone.Fourth)]
            },

            // 翅: chi
            ['翅'] = new()
            {
                PinYins = [new("chi", CharTone.Fourth)]
            },

            // 辱: ru
            ['辱'] = new()
            {
                PinYins = [new("ru", CharTone.Third)]
            },

            // 唇: chun
            ['唇'] = new()
            {
                PinYins = [new("chun", CharTone.Second)]
            },

            // 夏: xia
            ['夏'] = new()
            {
                PinYins = [new("xia", CharTone.Fourth)]
            },

            // 础: chu
            ['础'] = new()
            {
                PinYins = [new("chu", CharTone.Third)]
            },

            // 破: po
            ['破'] = new()
            {
                PinYins = [new("po", CharTone.Fourth)]
            },

            // 原: yuan
            ['原'] = new()
            {
                PinYins = [new("yuan", CharTone.Second)]
            },

            // 套: tao
            ['套'] = new()
            {
                PinYins = [new("tao", CharTone.Fourth)]
            },

            // 逐: zhu
            ['逐'] = new()
            {
                PinYins = [new("zhu", CharTone.Second)]
            },

            // 烈: lie
            ['烈'] = new()
            {
                PinYins = [new("lie", CharTone.Fourth)]
            },

            // 殊: shu
            ['殊'] = new()
            {
                PinYins = [new("shu", CharTone.Second)]
            },

            // 顾: gu
            ['顾'] = new()
            {
                PinYins = [new("gu", CharTone.Fourth)]
            },

            // 轿: jiao
            ['轿'] = new()
            {
                PinYins = [new("jiao", CharTone.Fourth)]
            },

            // 较: jiao
            ['较'] = new()
            {
                PinYins = [new("jiao", CharTone.Fourth)]
            },

            // 顿: dun | du
            ['顿'] = new()
            {
                PinYins = [
                    new("dun", CharTone.Fourth),
                    new("du", CharTone.Second, cases: ["冒顿"])
                ]
            },

            // 毙: bi
            ['毙'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 致: zhi
            ['致'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 柴: chai
            ['柴'] = new()
            {
                PinYins = [new("chai", CharTone.Second)]
            },

            // 桌: zhuo
            ['桌'] = new()
            {
                PinYins = [new("zhuo", CharTone.First)]
            }
        };
    }
}
