namespace com.etsoo.Localization.Chinese
{
    /// <summary>
    /// Chinese characters
    /// 中文字符，查询百度“奇 多音字”
    /// https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=%E5%A5%87%20%E5%A4%9A%E9%9F%B3%E5%AD%97&fenlei=256&oq=%25E5%25A5%25BD%2520%25E5%25A4%259A%25E9%259F%25B3%25E5%25AD%2597&rsv_pq=a381ac69002c8793&rsv_t=70f8v87SQ9opqNw20bnx3d7rjQFqmlMEFUkD8XoUmYZg04ygfkpvA2R7Nd0&rqlang=cn&rsv_dl=tb&rsv_enter=0&rsv_btype=t&rsv_sug3=91&rsv_sug1=23&rsv_sug7=100&rsv_sug2=0&inputT=1744&rsv_sug4=1744
    /// </summary>
    internal static partial class ChineseCharacters
    {
        /// <summary>
        /// All Chinese characters 1
        /// 所有中文字符1
        /// </summary>
        public static readonly SortedDictionary<char, ChineseCharacter> Characters1 = new()
        {
            // 一: yi
            ['一'] = new()
            {
                PinYins = [new("yi", CharTone.First)]
            },

            // 乙: yi
            ['乙'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 二: er
            ['二'] = new()
            {
                PinYins = [new("er", CharTone.Fourth)]
            },

            // 十: shi
            ['十'] = new()
            {
                PinYins = [new("shi", CharTone.Second)]
            },

            // 丁: ding | zheng
            ['丁'] = new()
            {
                PinYins = [
                    new("ding", CharTone.First, isFamilyName: true),
                    new("zheng", CharTone.First, cases: ["丁丁"])
                ]
            },

            // 厂: chang
            ['厂'] = new()
            {
                PinYins = [new("chang", CharTone.Third)]
            },

            // 七: qi
            ['七'] = new()
            {
                PinYins = [new("qi", CharTone.First)]
            },

            // 卜: bu | bo
            ['卜'] = new()
            {
                PinYins = [
                    new("bu", CharTone.Third, isFamilyName: true),
                    new("bo", CharTone.First, cases: ["萝卜"])
                ]
            },

            // 人: ren
            ['人'] = new()
            {
                PinYins = [new("ren", CharTone.Second)]
            },

            // 入: ru
            ['入'] = new()
            {
                PinYins = [new("ru", CharTone.Fourth)]
            },

            // 八: ba
            ['八'] = new()
            {
                PinYins = [new("ba", CharTone.First)]
            },

            // 九: jiu
            ['九'] = new()
            {
                PinYins = [new("jiu", CharTone.Third)]
            },

            // 几: ji
            ['几'] = new()
            {
                PinYins = [
                    new("ji", CharTone.Third),
                    new("ji", CharTone.First, cases: ["几乎", "几率", "条几", "茶几", "案几", "凭几", "研几", "几希", "净几", "几净", "几亮"])
                ]
            },

            // 儿: er
            ['儿'] = new()
            {
                PinYins = [new("er", CharTone.Second)]
            },

            // 了: liao | le
            ['了'] = new()
            {
                PinYins = [
                    new("liao", CharTone.Third),
                    new("le", CharTone.First)
                ]
            },

            // 力: li
            ['力'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 乃: nai
            ['乃'] = new()
            {
                PinYins = [new("nai", CharTone.Third)]
            },

            // 刀: dao
            ['刀'] = new()
            {
                PinYins = [new("dao", CharTone.First)]
            },

            // 又: you
            ['又'] = new()
            {
                PinYins = [new("you", CharTone.Fourth)]
            },

            // 三: san
            ['三'] = new()
            {
                PinYins = [new("san", CharTone.First)]
            },

            // 于: yu
            ['于'] = new()
            {
                PinYins = [new("yu", CharTone.Second, isFamilyName: true)]
            },

            // 干: gan(1) | gan(4)
            ['干'] = new()
            {
                PinYins = [
                    new("gan", CharTone.First, isFamilyName: true),
                    new("gan", CharTone.Fourth)
                ]
            },

            // 亏: kui
            ['亏'] = new()
            {
                PinYins = [new("kui", CharTone.First)]
            },

            // 士: shi
            ['士'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 工: gong
            ['工'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 土: tu
            ['土'] = new()
            {
                PinYins = [new("tu", CharTone.Third)]
            },

            // 才: cai
            ['才'] = new()
            {
                PinYins = [new("cai", CharTone.Second)]
            },

            // 寸: cun
            ['寸'] = new()
            {
                PinYins = [new("cun", CharTone.Fourth)]
            },

            // 下: xia
            ['下'] = new()
            {
                PinYins = [new("xia", CharTone.Fourth)]
            },

            // 大: da | dai
            ['大'] = new()
            {
                PinYins = [
                    new("da", CharTone.Fourth),
                    new("dai", CharTone.Fourth, cases: ["大夫"])
                ]
            },

            // 丈: zhang
            ['丈'] = new()
            {
                PinYins = [new("zhang", CharTone.Fourth)]
            },

            // 与: yu
            ['与'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 万: wan | mo
            ['万'] = new()
            {
                PinYins = [
                    new("wan", CharTone.Fourth, isFamilyName: true),
                    new("mo", CharTone.Second, cases: ["万俟"])
                ]
            },

            // 上: shang
            ['上'] = new()
            {
                PinYins = [
                    new("shang", CharTone.Fourth),
                    new("shang", CharTone.Third, cases: ["上声"])
                ]
            },

            // 小: xiao
            ['小'] = new()
            {
                PinYins = [new("xiao", CharTone.Third)]
            },

            // 口: kou
            ['口'] = new()
            {
                PinYins = [new("kou", CharTone.Third)]
            },

            // 巾: jin
            ['巾'] = new()
            {
                PinYins = [new("jin", CharTone.First)]
            },

            // 山: shan
            ['山'] = new()
            {
                PinYins = [new("shan", CharTone.First)]
            },

            // 千: qian
            ['千'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 乞: qi
            ['乞'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 川: chuan
            ['川'] = new()
            {
                PinYins = [new("chuan", CharTone.First)]
            },

            // 亿: yi
            ['亿'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 个: ge
            ['个'] = new()
            {
                PinYins = [new("ge", CharTone.Fourth)]
            },

            // 勺: shao
            ['勺'] = new()
            {
                PinYins = [new("shao", CharTone.Second)]
            },

            // 久: jiu
            ['久'] = new()
            {
                PinYins = [new("jiu", CharTone.Third)]
            },

            // 凡: fan
            ['凡'] = new()
            {
                PinYins = [new("fan", CharTone.Second)]
            },

            // 及: ji
            ['及'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 夕: xi
            ['夕'] = new()
            {
                PinYins = [new("xi", CharTone.First)]
            },

            // 丸: wan
            ['丸'] = new()
            {
                PinYins = [new("wan", CharTone.Second)]
            },

            // 么: me
            ['么'] = new()
            {
                PinYins = [new("me", CharTone.First)]
            },

            // 广: guang
            ['广'] = new()
            {
                PinYins = [new("guang", CharTone.Third)]
            },

            // 亡: wang
            ['亡'] = new()
            {
                PinYins = [new("wang", CharTone.Second)]
            },

            // 门: men
            ['门'] = new()
            {
                PinYins = [new("men", CharTone.Second)]
            },

            // 义: yi
            ['义'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 之: zhi
            ['之'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 尸: shi
            ['尸'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },


            // 弓: gong
            ['弓'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 己: ji
            ['己'] = new()
            {
                PinYins = [new("ji", CharTone.Third)]
            },

            // 已: yi
            ['已'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 子: zi
            ['子'] = new()
            {
                PinYins = [new("zi", CharTone.Third)]
            },

            // 卫: wei
            ['卫'] = new()
            {
                PinYins = [new("wei", CharTone.Fourth, isFamilyName: true)]
            },

            // 也: ye
            ['也'] = new()
            {
                PinYins = [new("ye", CharTone.Third)]
            },

            // 女: nv
            ['女'] = new()
            {
                PinYins = [new("nv", CharTone.Third)]
            },

            // 飞: fei
            ['飞'] = new()
            {
                PinYins = [new("fei", CharTone.First)]
            },

            // 刃: ren
            ['刃'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 习: xi
            ['习'] = new()
            {
                PinYins = [new("xi", CharTone.Second, isFamilyName: true)]
            },

            // 叉: cha
            ['叉'] = new()
            {
                PinYins = [new("cha", CharTone.First)]
            },

            // 马: ma
            ['马'] = new()
            {
                PinYins = [new("ma", CharTone.Third)]
            },

            // 乡: xiang
            ['乡'] = new()
            {
                PinYins = [new("xiang", CharTone.First)]
            },

            // 丰: feng
            ['丰'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 王: wang
            ['王'] = new()
            {
                PinYins = [new("wang", CharTone.Second, isFamilyName: true)]
            },

            // 井: jing
            ['井'] = new()
            {
                PinYins = [new("jing", CharTone.Third)]
            },

            // 开: kai
            ['开'] = new()
            {
                PinYins = [new("kai", CharTone.First)]
            },

            // 夫: fu
            ['夫'] = new()
            {
                PinYins = [new("fu", CharTone.First)]
            },

            // 天: tian
            ['天'] = new()
            {
                PinYins = [new("tian", CharTone.First)]
            },

            // 无: wu | mo
            ['无'] = new()
            {
                PinYins = [
                    new("wu", CharTone.Second),
                    new("mo", CharTone.Second, cases: ["南无"])
                ]
            },

            // 元: yuan
            ['元'] = new()
            {
                PinYins = [new("yuan", CharTone.Second)]
            },

            // 专: zhuan
            ['专'] = new()
            {
                PinYins = [new("zhuan", CharTone.First)]
            },

            // 云: yun
            ['云'] = new()
            {
                PinYins = [new("yun", CharTone.Second)]
            },

            // 扎: za | zha
            ['扎'] = new()
            {
                PinYins = [
                    new("za", CharTone.First),
                    new("zha", CharTone.First, cases: ["扎根", "扎实", "扎针", "驻扎", "扎花"]),
                    new("zha", CharTone.Second, cases: ["挣扎"])
                ]
            },

            // 艺: yi
            ['艺'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 木: mu
            ['木'] = new()
            {
                PinYins = [new("mu", CharTone.Fourth)]
            },

            // 五: wu
            ['五'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 支: zhi
            ['支'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 厅: ting
            ['厅'] = new()
            {
                PinYins = [new("ting", CharTone.First)]
            },

            // 不: bu
            ['不'] = new()
            {
                PinYins = [new("bu", CharTone.Fourth)]
            },

            // 太: tai
            ['太'] = new()
            {
                PinYins = [new("tai", CharTone.Fourth)]
            },

            // 犬: quan
            ['犬'] = new()
            {
                PinYins = [new("quan", CharTone.Third)]
            },

            // 区: qu | ou
            ['区'] = new()
            {
                PinYins = [
                    new("qu", CharTone.First),
                    new("ou", CharTone.First, isFamilyName: true)
                ]
            },

            // 历: li
            ['历'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 尤: you
            ['尤'] = new()
            {
                PinYins = [new("you", CharTone.Second)]
            },

            // 友: you
            ['友'] = new()
            {
                PinYins = [new("you", CharTone.Third)]
            },

            // 匹: pi
            ['匹'] = new()
            {
                PinYins = [new("pi", CharTone.Third)]
            },

            // 车: che | ju
            ['车'] = new()
            {
                PinYins = [
                    new("che", CharTone.First),
                    new("ju", CharTone.Fourth, cases: ["弃车保帅", "舍车保帅"])
                ]
            },

            // 巨: ju
            ['巨'] = new()
            {
                PinYins = [new("ju", CharTone.Fourth)]
            },

            // 牙: ya
            ['牙'] = new()
            {
                PinYins = [new("ya", CharTone.Second)]
            },

            // 屯: tun | zhun
            ['屯'] = new()
            {
                PinYins = [
                    new("tun", CharTone.Second),
                    new("zhun", CharTone.First, cases: ["屯穷", "屯钝", "屯闵", "屯膏"])
                ]
            },

            // 比: bi
            ['比'] = new()
            {
                PinYins = [new("bi", CharTone.Third)]
            },

            // 互: hu
            ['互'] = new()
            {
                PinYins = [new("hu", CharTone.Fourth)]
            },

            // 切: qie
            ['切'] = new()
            {
                PinYins = [new("qie", CharTone.First)]
            },

            // 瓦: wa
            ['瓦'] = new()
            {
                PinYins = [new("wa", CharTone.Third)]
            },

            // 止: zhi
            ['止'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 少: shao
            ['少'] = new()
            {
                PinYins = [
                    new("shao", CharTone.Fourth),
                    new("shao", CharTone.Third)
                ]
            },

            // 日: ri
            ['日'] = new()
            {
                PinYins = [new("ri", CharTone.Fourth)]
            },

            // 中: zhong
            ['中'] = new()
            {
                PinYins = [new("zhong", CharTone.First)]
            },

            // 冈: gang
            ['冈'] = new()
            {
                PinYins = [new("gang", CharTone.First)]
            },

            // 贝: bei
            ['贝'] = new()
            {
                PinYins = [new("bei", CharTone.Fourth)]
            },

            // 内: nei
            ['内'] = new()
            {
                PinYins = [new("nei", CharTone.Fourth)]
            },

            // 水: shui
            ['水'] = new()
            {
                PinYins = [new("shui", CharTone.Third)]
            },

            // 见: jian | xian
            ['见'] = new()
            {
                PinYins = [
                    new("jian", CharTone.Fourth),
                    new("xian", CharTone.Fourth, cases: ["初见", "情见", "叠见", "肘见", "层见", "龙见", "见粮", "见世报", "图穷匕见", "见世生苗", "间见层出", "水清石见", "见素抱朴"])
                ]
            },

            // 午: wu
            ['午'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 牛: niu
            ['牛'] = new()
            {
                PinYins = [new("niu", CharTone.Second)]
            },

            // 手: shou
            ['手'] = new()
            {
                PinYins = [new("shou", CharTone.Third)]
            },

            // 毛: mao
            ['毛'] = new()
            {
                PinYins = [new("mao", CharTone.Second)]
            },

            // 气: qi
            ['气'] = new()
            {
                PinYins = [new("qi", CharTone.Fourth)]
            },

            // 升: sheng
            ['升'] = new()
            {
                PinYins = [new("sheng", CharTone.First)]
            },

            // 长: chang | zhang
            ['长'] = new()
            {
                PinYins = [
                    new("chang", CharTone.Second, cases: ["长沙"]),
                    new("zhang", CharTone.Third, isFamilyName: true, cases: ["学长", "生长", "成长", "家长", "连长", "长老", "师长", "队长", "校长", "首长", "族长", "科长", "市长", "厂长", "州长", "助长", "兄长", "院长", "长膘", "长进", "助长", "长大", "长幼", "总长", "长官", "土长", "长子", "长女", "增长", "见长", "相长", "股长", "董事长", "保长", "警长", "社长", "县长", "省长", "部长", "百夫长", "护士长", "排长", "组长", "道长", "行长", "会长", "镇长", "区长", "营长", "长幼", "尊长", "滋长", "长孙", "长者", "长相", "议长", "厅长", "消长", "长势", "长房", "长辈", "司务长", "检察长", "长史", "年长", "长兄", "长嫂", "草长莺飞"])
                ]
            },

            // 仁: ren
            ['仁'] = new()
            {
                PinYins = [new("ren", CharTone.Second)]
            },

            // 什: shi | shen
            ['什'] = new()
            {
                PinYins = [
                    new("shi", CharTone.Second),
                    new("shen", CharTone.Second, cases: ["什锦", "什器", "什物"])
                ]
            },

            // 片: pian
            ['片'] = new()
            {
                PinYins = [new("pian", CharTone.Fourth)]
            },


            // 仆: pu
            ['仆'] = new()
            {
                PinYins = [
                    new("pu", CharTone.First),
                    new("pu", CharTone.Second, cases: ["仆人", "仆从", "男仆", "女仆", "仆役", "太仆", "仆射"])
                ]
            },

            // 化: hua
            ['化'] = new()
            {
                PinYins = [new("hua", CharTone.Fourth)]
            },

            // 仇: chou | qiu
            ['仇'] = new()
            {
                PinYins = [
                    new("chou", CharTone.Second),
                    new("qiu", CharTone.Second, isFamilyName: true)
                ]
            },

            // 币: bi
            ['币'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 仍: reng
            ['仍'] = new()
            {
                PinYins = [new("reng", CharTone.Second)]
            },

            // 仅: jin
            ['仅'] = new()
            {
                PinYins = [new("jin", CharTone.Third)]
            },

            // 斤: jin
            ['斤'] = new()
            {
                PinYins = [new("jin", CharTone.First)]
            },

            // 爪: zhao | zhua
            ['爪'] = new()
            {
                PinYins = [
                    new("zhao", CharTone.Third, cases: ["鹰爪", "虎爪", "张牙舞爪"]),
                    new("zhua", CharTone.Third)
                ]
            },

            // 反: fan
            ['反'] = new()
            {
                PinYins = [new("fan", CharTone.Third)]
            },

            // 介: jie
            ['介'] = new()
            {
                PinYins = [new("jie", CharTone.Fourth)]
            },

            // 父: fu
            ['父'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 从: cong
            ['从'] = new()
            {
                PinYins = [new("cong", CharTone.Second)]
            },

            // 今: jin
            ['今'] = new()
            {
                PinYins = [new("jin", CharTone.First)]
            },

            // 凶: xiong
            ['凶'] = new()
            {
                PinYins = [new("xiong", CharTone.First)]
            },

            // 分: fen
            ['分'] = new()
            {
                PinYins = [new("fen", CharTone.First)]
            },

            // 乏: fa
            ['乏'] = new()
            {
                PinYins = [new("fa", CharTone.Second)]
            },

            // 公: gong
            ['公'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 仓: cang
            ['仓'] = new()
            {
                PinYins = [new("cang", CharTone.First)]
            },

            // 月: yue
            ['月'] = new()
            {
                PinYins = [new("yue", CharTone.Fourth)]
            },

            // 氏: shi | zhi
            ['氏'] = new()
            {
                PinYins = [
                    new("shi", CharTone.Fourth),
                    new("zhi", CharTone.First, cases: ["阏氏", "月氏"])
                ]
            },

            // 勿: wu
            ['勿'] = new()
            {
                PinYins = [new("wu", CharTone.Fourth)]
            },

            // 欠: qian
            ['欠'] = new()
            {
                PinYins = [new("qian", CharTone.Fourth)]
            },

            // 风: feng
            ['风'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 丹: dan
            ['丹'] = new()
            {
                PinYins = [new("dan", CharTone.First)]
            },

            // 匀: yun
            ['匀'] = new()
            {
                PinYins = [new("yun", CharTone.Second)]
            },

            // 乌: wu
            ['乌'] = new()
            {
                PinYins = [new("wu", CharTone.First)]
            },

            // 凤: feng
            ['凤'] = new()
            {
                PinYins = [new("feng", CharTone.Fourth)]
            },

            // 勾: gou
            ['勾'] = new()
            {
                PinYins = [new("gou", CharTone.First)]
            },

            // 文: wen
            ['文'] = new()
            {
                PinYins = [new("wen", CharTone.Second)]
            },

            // 六: liu
            ['六'] = new()
            {
                PinYins = [new("liu", CharTone.Fourth)]
            },

            // 方: fang
            ['方'] = new()
            {
                PinYins = [new("fang", CharTone.First)]
            },

            // 火: huo
            ['火'] = new()
            {
                PinYins = [new("huo", CharTone.Third)]
            },

            // 为: wei
            ['为'] = new()
            {
                PinYins = [new("wei", CharTone.Second)]
            },

            // 斗: dou
            ['斗'] = new()
            {
                PinYins = [
                    new("dou", CharTone.Fourth),
                    new("dou", CharTone.Third, cases: ["筋斗", "斗胆", "北斗", "斗笠", "斗篷", "阿斗", "斗金", "烟斗", "大斗", "八斗", "一斗", "两斗", "三斗", "五斗", "小斗", "翻跟斗", "斗转星移"])
                ]
            },

            // 忆: yi
            ['忆'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 订: ding
            ['订'] = new()
            {
                PinYins = [new("ding", CharTone.Fourth)]
            },

            // 计: ji
            ['计'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 户: hu
            ['户'] = new()
            {
                PinYins = [new("hu", CharTone.Fourth)]
            },

            // 认: ren
            ['认'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 心: xin
            ['心'] = new()
            {
                PinYins = [new("xin", CharTone.First)]
            },

            // 尺: chi
            ['尺'] = new()
            {
                PinYins = [new("chi", CharTone.Third)]
            },

            // 引: yin
            ['引'] = new()
            {
                PinYins = [new("yin", CharTone.Third)]
            },

            // 丑: chou
            ['丑'] = new()
            {
                PinYins = [new("chou", CharTone.Third)]
            },

            // 巴: ba
            ['巴'] = new()
            {
                PinYins = [new("ba", CharTone.First)]
            },

            // 孔: kong
            ['孔'] = new()
            {
                PinYins = [new("kong", CharTone.Third)]
            },

            // 队: dui
            ['队'] = new()
            {
                PinYins = [new("dui", CharTone.Fourth)]
            },

            // 办: ban
            ['办'] = new()
            {
                PinYins = [new("ban", CharTone.Fourth)]
            },

            // 以: yi
            ['以'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 允: yun
            ['允'] = new()
            {
                PinYins = [new("yun", CharTone.Third)]
            },

            // 予: yu
            ['予'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 劝: quan
            ['劝'] = new()
            {
                PinYins = [new("quan", CharTone.Fourth)]
            },

            // 双: shuang
            ['双'] = new()
            {
                PinYins = [new("shuang", CharTone.First)]
            },

            // 书: shu
            ['书'] = new()
            {
                PinYins = [new("shu", CharTone.First)]
            },

            // 幻: huan
            ['幻'] = new()
            {
                PinYins = [new("huan", CharTone.Fourth)]
            },

            // 玉: yu
            ['玉'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 刊: kan
            ['刊'] = new()
            {
                PinYins = [new("kan", CharTone.First)]
            },

            // 示: shi
            ['示'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 末: mo
            ['末'] = new()
            {
                PinYins = [new("mo", CharTone.Fourth)]
            },

            // 未: wei
            ['未'] = new()
            {
                PinYins = [new("wei", CharTone.Fourth)]
            },

            // 击: ji
            ['击'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 打: da
            ['打'] = new()
            {
                PinYins = [new("da", CharTone.Third)]
            },

            // 巧: qiao
            ['巧'] = new()
            {
                PinYins = [new("qiao", CharTone.Third)]
            },

            // 正: zheng
            ['正'] = new()
            {
                PinYins = [
                    new("zheng", CharTone.Fourth),
                    new("zheng", CharTone.First, cases: ["正月", "正旦", "正朔"])
                ]
            },

            // 扑: pu
            ['扑'] = new()
            {
                PinYins = [new("pu", CharTone.First)]
            },

            // 扒: ba | pa
            ['扒'] = new()
            {
                PinYins = [
                    new("ba", CharTone.First),
                    new("pa", CharTone.Second)
                ]
            },

            // 功: gong
            ['功'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 扔: reng
            ['扔'] = new()
            {
                PinYins = [new("reng", CharTone.First)]
            },

            // 去: qu
            ['去'] = new()
            {
                PinYins = [new("qu", CharTone.Fourth)]
            },

            // 甘: gan
            ['甘'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 世: shi
            ['世'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 古: gu
            ['古'] = new()
            {
                PinYins = [new("gu", CharTone.Third)]
            },

            // 节: jie
            ['节'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 本: ben
            ['本'] = new()
            {
                PinYins = [new("ben", CharTone.Third)]
            },

            // 术: shu | zhu
            ['术'] = new()
            {
                PinYins = [
                    new("shu", CharTone.Fourth),
                    new("zhu", CharTone.Second, cases: ["金兀术"])
                ]
            },

            // 可: ke
            ['可'] = new()
            {
                PinYins = [new("ke", CharTone.Third)]
            },

            // 丙: bing
            ['丙'] = new()
            {
                PinYins = [new("bing", CharTone.Third)]
            },

            // 左: zuo
            ['左'] = new()
            {
                PinYins = [new("zuo", CharTone.Third)]
            },

            // 厉: li
            ['厉'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 右: you
            ['右'] = new()
            {
                PinYins = [new("you", CharTone.Fourth)]
            },

            // 石: shi | dan
            ['石'] = new()
            {
                PinYins = [
                    new("shi", CharTone.Second),
                    new("dan", CharTone.Fourth, cases: ["一石", "十石"])
                ]
            },

            // 布: bu
            ['布'] = new()
            {
                PinYins = [new("bu", CharTone.Fourth)]
            },

            // 龙: long
            ['龙'] = new()
            {
                PinYins = [new("long", CharTone.Second)]
            },

            // 平: ping
            ['平'] = new()
            {
                PinYins = [new("ping", CharTone.Second)]
            },

            // 灭: mie
            ['灭'] = new()
            {
                PinYins = [new("mie", CharTone.Fourth)]
            },

            // 轧: ya | zha | ga
            ['轧'] = new()
            {
                PinYins = [
                    new("ya", CharTone.Fourth),
                    new("zha", CharTone.Second, cases: ["轧钢", "轧辊", "轧机", "轧制"]),
                    new("ga", CharTone.Second, cases: ["轧账", "轧平", "轧不平", "轧朋友", "轧轧"])
                ]
            },

            // 东: dong
            ['东'] = new()
            {
                PinYins = [new("dong", CharTone.First)]
            },

            // 卡: ka | qia
            ['卡'] = new()
            {
                PinYins = [
                    new("ka", CharTone.Third),
                    new("qia", CharTone.Third, cases: ["卡具", "哨卡", "边卡", "卡壳", "发卡", "关卡", "税卡", "岗卡", "卡子", "路卡", "暗卡", "卡脖"])
                ]
            },

            // 北: bei
            ['北'] = new()
            {
                PinYins = [new("bei", CharTone.Third)]
            },

            // 占: zhan
            ['占'] = new()
            {
                PinYins = [
                    new("zhan", CharTone.Fourth),
                    new("zhan", CharTone.First, cases: ["占卜", "占星", "占卦", "占占", "占占卜卜"])
                ]
            },

            // 业: ye
            ['业'] = new()
            {
                PinYins = [new("ye", CharTone.Fourth)]
            },

            // 旧: jiu
            ['旧'] = new()
            {
                PinYins = [new("jiu", CharTone.Fourth)]
            },

            // 帅: shuai
            ['帅'] = new()
            {
                PinYins = [new("shuai", CharTone.Fourth)]
            },

            // 归: gui
            ['归'] = new()
            {
                PinYins = [new("gui", CharTone.First)]
            },

            // 且: qie
            ['且'] = new()
            {
                PinYins = [new("qie", CharTone.Third)]
            },

            // 旦: dan
            ['旦'] = new()
            {
                PinYins = [new("dan", CharTone.Fourth)]
            },

            // 目: mu
            ['目'] = new()
            {
                PinYins = [new("mu", CharTone.Fourth)]
            },

            // 叶: ye
            ['叶'] = new()
            {
                PinYins = [new("ye", CharTone.Fourth)]
            },

            // 甲: jia
            ['甲'] = new()
            {
                PinYins = [new("jia", CharTone.Third)]
            },

            // 申: shen
            ['申'] = new()
            {
                PinYins = [new("shen", CharTone.First)]
            },

            // 叮: ding
            ['叮'] = new()
            {
                PinYins = [new("ding", CharTone.First)]
            },

            // 电: dian
            ['电'] = new()
            {
                PinYins = [new("dian", CharTone.Fourth)]
            },

            // 号: hao
            ['号'] = new()
            {
                PinYins = [
                    new("hao", CharTone.Fourth),
                    new("hao", CharTone.Second, cases: ["怒号", "号啕", "哀号", "号哭", "号丧", "号叫", "号天", "号泣", "风号", "啼号", "鬼哭狼号"])
                ]
            },

            // 田: tian
            ['田'] = new()
            {
                PinYins = [new("tian", CharTone.Second)]
            },

            // 由: you
            ['由'] = new()
            {
                PinYins = [new("you", CharTone.Second)]
            },

            // 史: shi
            ['史'] = new()
            {
                PinYins = [new("shi", CharTone.Third)]
            },

            // 只: zhi
            ['只'] = new()
            {
                PinYins = [
                    new("zhi", CharTone.First),
                    new("zhi", CharTone.Third, cases: ["只有", "只是", "只见", "只能", "只要", "只管", "只顾", "只好"])
                ]
            },

            // 央: yang
            ['央'] = new()
            {
                PinYins = [new("yang", CharTone.First)]
            },

            // 兄: xiong
            ['兄'] = new()
            {
                PinYins = [new("xiong", CharTone.First)]
            },

            // 叼: diao
            ['叼'] = new()
            {
                PinYins = [new("diao", CharTone.First)]
            },

            // 叫: jiao
            ['叫'] = new()
            {
                PinYins = [new("jiao", CharTone.Fourth)]
            },

            // 另: ling
            ['另'] = new()
            {
                PinYins = [new("ling", CharTone.Fourth)]
            },

            // 叨: dao | tao
            ['叨'] = new()
            {
                PinYins = [
                    new("dao", CharTone.First),
                    new("tao", CharTone.First, cases: ["叨扰"])
                ]
            },

            // 叹: tan
            ['叹'] = new()
            {
                PinYins = [new("tan", CharTone.Fourth)]
            },

            // 四: si
            ['四'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 生: sheng
            ['生'] = new()
            {
                PinYins = [new("sheng", CharTone.First)]
            },

            // 失: shi
            ['失'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },

            // 禾: he
            ['禾'] = new()
            {
                PinYins = [new("he", CharTone.Second)]
            },

            // 丘: qiu
            ['丘'] = new()
            {
                PinYins = [new("qiu", CharTone.First)]
            },

            // 付: fu
            ['付'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 仗: zhang
            ['仗'] = new()
            {
                PinYins = [new("zhang", CharTone.Fourth)]
            },

            // 代: dai
            ['代'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 仙: xian
            ['仙'] = new()
            {
                PinYins = [new("xian", CharTone.First)]
            },

            // 们: men
            ['们'] = new()
            {
                PinYins = [new("men", CharTone.Second)]
            },

            // 仪: yi
            ['仪'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 白: bai
            ['白'] = new()
            {
                PinYins = [new("bai", CharTone.Second)]
            },

            // 仔: zi | zai
            ['仔'] = new()
            {
                PinYins = [
                    new("zi", CharTone.Third),
                    new("zai", CharTone.Third, cases: ["马仔", "番仔", "牛仔裤"])
                ]
            },

            // 他: ta
            ['他'] = new()
            {
                PinYins = [new("ta", CharTone.First)]
            },

            // 斥: chi
            ['斥'] = new()
            {
                PinYins = [new("chi", CharTone.Fourth)]
            },

            // 瓜: gua
            ['瓜'] = new()
            {
                PinYins = [new("gua", CharTone.First)]
            },

            // 乎: hu
            ['乎'] = new()
            {
                PinYins = [new("hu", CharTone.First)]
            },

            // 丛: cong
            ['丛'] = new()
            {
                PinYins = [new("cong", CharTone.Second)]
            },

            // 令: ling
            ['令'] = new()
            {
                PinYins = [new("ling", CharTone.Fourth)]
            },

            // 用: yong
            ['用'] = new()
            {
                PinYins = [new("yong", CharTone.Fourth)]
            },

            // 甩: shuai
            ['甩'] = new()
            {
                PinYins = [new("shuai", CharTone.Third)]
            },

            // 印: yin
            ['印'] = new()
            {
                PinYins = [new("yin", CharTone.Fourth)]
            },

            // 乐: le | yue
            ['乐'] = new()
            {
                PinYins = [
                    new("le", CharTone.Fourth),
                    new("yue", CharTone.Fourth, cases: ["音乐", "声乐", "乐池", "乐器"])
                ]
            },

            // 句: ju | gou
            ['句'] = new()
            {
                PinYins = [
                    new("ju", CharTone.Fourth),
                    new("gou", CharTone.First, cases: ["高句丽", "句芒", "钩章棘句", "句吴", "句望", "句萌", "句骊", "句枉"])
                ]
            },

            // 匆: cong
            ['匆'] = new()
            {
                PinYins = [new("cong", CharTone.First)]
            },

            // 册: ce
            ['册'] = new()
            {
                PinYins = [new("ce", CharTone.Fourth)]
            },

            // 犯: fan
            ['犯'] = new()
            {
                PinYins = [new("fan", CharTone.Fourth)]
            },

            // 外: wai
            ['外'] = new()
            {
                PinYins = [new("wai", CharTone.Fourth)]
            },

            // 处: chu
            ['处'] = new()
            {
                PinYins = [
                    new("chu", CharTone.Fourth),
                    new("chu", CharTone.Third, cases: ["处境", "处女", "处分", "处方", "处世", "处理", "处置", "处罚", "处死", "共处", "相处", "论处", "处不来", "处心积虑", "身处逆境", "处变不惊", "处之泰然", "为人处世"])
                ]
            },

            // 冬: dong
            ['冬'] = new()
            {
                PinYins = [new("dong", CharTone.First)]
            },

            // 鸟: niao | diao
            ['鸟'] = new()
            {
                PinYins = [
                    new("niao", CharTone.Third),
                    new("diao", CharTone.Third, cases: ["鸟乱", "鸟事", "鸟样"])
                ]
            },

            // 务: wu
            ['务'] = new()
            {
                PinYins = [new("wu", CharTone.Fourth)]
            },

            // 包: bao
            ['包'] = new()
            {
                PinYins = [new("bao", CharTone.First)]
            },

            // 饥: ji
            ['饥'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 主: zhu
            ['主'] = new()
            {
                PinYins = [new("zhu", CharTone.Third)]
            },

            // 市: shi
            ['市'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 立: li
            ['立'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 闪: shan
            ['闪'] = new()
            {
                PinYins = [new("shan", CharTone.Third)]
            },

            // 兰: lan
            ['兰'] = new()
            {
                PinYins = [new("lan", CharTone.Second)]
            },

            // 半: ban
            ['半'] = new()
            {
                PinYins = [new("ban", CharTone.Fourth)]
            },

            // 汁: zhi
            ['汁'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 汇: hui
            ['汇'] = new()
            {
                PinYins = [new("hui", CharTone.Fourth)]
            },

            // 头: tou
            ['头'] = new()
            {
                PinYins = [new("tou", CharTone.Second)]
            },

            // 汉: han
            ['汉'] = new()
            {
                PinYins = [new("han", CharTone.Fourth)]
            },

            // 宁: ning
            ['宁'] = new()
            {
                PinYins = [
                    new("ning", CharTone.Second),
                    new("ning", CharTone.Fourth, cases: ["毋宁", "宁死", "宁当", "宁折不屈", "宁缺毋滥", "不宁唯是"])
                ]
            },

            // 穴: xue
            ['穴'] = new()
            {
                PinYins = [new("xue", CharTone.Fourth)]
            },

            // 它: ta
            ['它'] = new()
            {
                PinYins = [new("ta", CharTone.First)]
            },

            // 讨: tao
            ['讨'] = new()
            {
                PinYins = [new("tao", CharTone.Third)]
            },

            // 写: xie
            ['写'] = new()
            {
                PinYins = [new("xie", CharTone.Third)]
            },

            // 让: rang
            ['让'] = new()
            {
                PinYins = [new("rang", CharTone.Fourth)]
            },

            // 礼: li
            ['礼'] = new()
            {
                PinYins = [new("li", CharTone.Third)]
            },

            // 训: xun
            ['训'] = new()
            {
                PinYins = [new("xun", CharTone.Fourth)]
            },

            // 必: bi
            ['必'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 议: yi
            ['议'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 讯: xun
            ['讯'] = new()
            {
                PinYins = [new("xun", CharTone.Fourth)]
            },

            // 记: ji
            ['记'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 永: yong
            ['永'] = new()
            {
                PinYins = [new("yong", CharTone.Third)]
            },

            // 司: si
            ['司'] = new()
            {
                PinYins = [new("si", CharTone.First)]
            },

            // 尼: ni
            ['尼'] = new()
            {
                PinYins = [new("ni", CharTone.Second)]
            },

            // 民: min
            ['民'] = new()
            {
                PinYins = [new("min", CharTone.Second)]
            },

            // 出: chu
            ['出'] = new()
            {
                PinYins = [new("chu", CharTone.First)]
            },

            // 辽: liao
            ['辽'] = new()
            {
                PinYins = [new("liao", CharTone.Second)]
            },

            // 奶: nai
            ['奶'] = new()
            {
                PinYins = [new("nai", CharTone.Third)]
            },

            // 奴: nu
            ['奴'] = new()
            {
                PinYins = [new("nu", CharTone.Second)]
            },

            // 加: jia
            ['加'] = new()
            {
                PinYins = [new("jia", CharTone.First)]
            },

            // 召: zhao | shao
            ['召'] = new()
            {
                PinYins = [
                    new("zhao", CharTone.Fourth),
                    new("shao", CharTone.Fourth, isFamilyName: true)
                ]
            },

            // 皮: pi
            ['皮'] = new()
            {
                PinYins = [new("pi", CharTone.Second)]
            },

            // 边: bian
            ['边'] = new()
            {
                PinYins = [new("bian", CharTone.First)]
            },

            // 发: fa
            ['发'] = new()
            {
                PinYins = [new("fa", CharTone.First)]
            },

            // 孕: yun
            ['孕'] = new()
            {
                PinYins = [new("yun", CharTone.Fourth)]
            },

            // 圣: sheng
            ['圣'] = new()
            {
                PinYins = [new("sheng", CharTone.Fourth)]
            },

            // 对: dui
            ['对'] = new()
            {
                PinYins = [new("dui", CharTone.Fourth)]
            },

            // 台: tai
            ['台'] = new()
            {
                PinYins = [new("tai", CharTone.Second)]
            },

            // 矛: mao
            ['矛'] = new()
            {
                PinYins = [new("mao", CharTone.Second)]
            },

            // 纠: jiu
            ['纠'] = new()
            {
                PinYins = [new("jiu", CharTone.First)]
            },

            // 母: mu
            ['母'] = new()
            {
                PinYins = [new("mu", CharTone.Third)]
            },

            // 幼: you
            ['幼'] = new()
            {
                PinYins = [new("you", CharTone.Fourth)]
            },

            // 丝: si
            ['丝'] = new()
            {
                PinYins = [new("si", CharTone.First)]
            },

            // 式: shi
            ['式'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 刑: xing
            ['刑'] = new()
            {
                PinYins = [new("xing", CharTone.Second)]
            },

            // 动: dong
            ['动'] = new()
            {
                PinYins = [new("dong", CharTone.Fourth)]
            },

            // 扛: kang | gang
            ['扛'] = new()
            {
                PinYins = [
                    new("kang", CharTone.Second),
                    new("gang", CharTone.First, cases: ["扛鼎"])
                ]
            },

            // 寺: si
            ['寺'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 吉: ji
            ['吉'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 扣: kou
            ['扣'] = new()
            {
                PinYins = [new("kou", CharTone.Fourth)]
            },

            // 考: kao
            ['考'] = new()
            {
                PinYins = [new("kao", CharTone.Third)]
            },

            // 托: tuo
            ['托'] = new()
            {
                PinYins = [new("tuo", CharTone.First)]
            },

            // 老: lao
            ['老'] = new()
            {
                PinYins = [new("lao", CharTone.Third)]
            },

            // 执: zhi
            ['执'] = new()
            {
                PinYins = [new("zhi", CharTone.Second)]
            },

            // 巩: gong
            ['巩'] = new()
            {
                PinYins = [new("gong", CharTone.Third)]
            },

            // 圾: ji
            ['圾'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 扩: kuo
            ['扩'] = new()
            {
                PinYins = [new("kuo", CharTone.Fourth)]
            },

            // 扫: sao
            ['扫'] = new()
            {
                PinYins = [new("sao", CharTone.Third)]
            },

            // 地: di | de
            ['地'] = new()
            {
                PinYins = [
                    new("di", CharTone.Fourth),
                    new("de", CharTone.First)
                ]
            },

            // 扬: yang
            ['扬'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 场: chang
            ['场'] = new()
            {
                PinYins = [
                    new("chang", CharTone.Third),
                    new("chang", CharTone.Second, cases: ["场院", "场屋", "打场", "禾场", "一场空", "起场", "灵场", "外场人"])
                ]
            },

            // 耳: er
            ['耳'] = new()
            {
                PinYins = [new("er", CharTone.Third)]
            },

            // 共: gong
            ['共'] = new()
            {
                PinYins = [new("gong", CharTone.Fourth)]
            },

            // 芒: mang
            ['芒'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 亚: ya
            ['亚'] = new()
            {
                PinYins = [new("ya", CharTone.Fourth)]
            },

            // 芝: zhi
            ['芝'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 朽: xiu
            ['朽'] = new()
            {
                PinYins = [new("xiu", CharTone.Third)]
            },

            // 朴: pu | po | piao
            ['朴'] = new()
            {
                PinYins = [
                    new("pu", CharTone.Third),
                    new("po", CharTone.First, cases: ["朴刀"]),
                    new("piao", CharTone.Second, isFamilyName: true)
                ]
            },

            // 机: ji
            ['机'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 权: quan
            ['权'] = new()
            {
                PinYins = [new("quan", CharTone.Second)]
            },

            // 过: guo
            ['过'] = new()
            {
                PinYins = [new("guo", CharTone.Fourth)]
            },

            // 臣: chen
            ['臣'] = new()
            {
                PinYins = [new("chen", CharTone.Second)]
            },

            // 再: zai
            ['再'] = new()
            {
                PinYins = [new("zai", CharTone.Fourth)]
            },

            // 协: xie
            ['协'] = new()
            {
                PinYins = [new("xie", CharTone.Second)]
            },

            // 西: xi
            ['西'] = new()
            {
                PinYins = [new("xi", CharTone.First)]
            },

            // 压: ya
            ['压'] = new()
            {
                PinYins = [new("ya", CharTone.First)]
            },

            // 厌: yan
            ['厌'] = new()
            {
                PinYins = [new("yan", CharTone.Fourth)]
            },

            // 在: zai
            ['在'] = new()
            {
                PinYins = [new("zai", CharTone.Fourth)]
            },

            // 有: you
            ['有'] = new()
            {
                PinYins = [new("you", CharTone.Third)]
            },

            // 百: bai
            ['百'] = new()
            {
                PinYins = [new("bai", CharTone.Third)]
            },

            // 存: cun
            ['存'] = new()
            {
                PinYins = [new("cun", CharTone.Second)]
            },

            // 而: er
            ['而'] = new()
            {
                PinYins = [new("er", CharTone.Second)]
            },

            // 页: ye
            ['页'] = new()
            {
                PinYins = [new("ye", CharTone.Fourth)]
            },

            // 匠: jiang
            ['匠'] = new()
            {
                PinYins = [new("jiang", CharTone.Fourth)]
            },

            // 夸: kua
            ['夸'] = new()
            {
                PinYins = [new("kua", CharTone.First)]
            },

            // 夺: duo
            ['夺'] = new()
            {
                PinYins = [new("duo", CharTone.Second)]
            },

            // 灰: hui
            ['灰'] = new()
            {
                PinYins = [new("hui", CharTone.First)]
            },

            // 达: da
            ['达'] = new()
            {
                PinYins = [new("da", CharTone.Second)]
            },

            // 列: lie
            ['列'] = new()
            {
                PinYins = [new("lie", CharTone.Fourth)]
            },

            // 死: si
            ['死'] = new()
            {
                PinYins = [new("si", CharTone.Third)]
            },

            // 成: cheng
            ['成'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 夹: jia | ga
            ['夹'] = new()
            {
                PinYins = [
                    new("jia", CharTone.First),
                    new("jia", CharTone.Second, cases: ["夹衣", "夹袄", "夹被", "夹鞋"]),
                    new("ga", CharTone.First, cases: ["夹肢窝"])
                ]
            },

            // 轨: gui
            ['轨'] = new()
            {
                PinYins = [new("gui", CharTone.Third)]
            },

            // 邪: xie | ye
            ['邪'] = new()
            {
                PinYins = [
                    new("xie", CharTone.Second),
                    new("ye", CharTone.Second)
                ]
            },

            // 划: hua
            ['划'] = new()
            {
                PinYins = [new("hua", CharTone.Second)]
            },

            // 迈: mai
            ['迈'] = new()
            {
                PinYins = [new("mai", CharTone.Fourth)]
            },

            // 毕: bi
            ['毕'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 至: zhi
            ['至'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 此: ci
            ['此'] = new()
            {
                PinYins = [new("ci", CharTone.Third)]
            },

            // 贞: zhen
            ['贞'] = new()
            {
                PinYins = [new("zhen", CharTone.First)]
            },

            // 师: shi
            ['师'] = new()
            {
                PinYins = [new("shi", CharTone.First)]
            },

            // 尘: chen
            ['尘'] = new()
            {
                PinYins = [new("chen", CharTone.Second)]
            },

            // 尖: jian
            ['尖'] = new()
            {
                PinYins = [new("jian", CharTone.First)]
            },

            // 劣: lie
            ['劣'] = new()
            {
                PinYins = [new("lie", CharTone.Fourth)]
            },

            // 光: guang
            ['光'] = new()
            {
                PinYins = [new("guang", CharTone.First)]
            },

            // 当: dang
            ['当'] = new()
            {
                PinYins = [new("dang", CharTone.First)]
            },

            // 早: zao
            ['早'] = new()
            {
                PinYins = [new("zao", CharTone.Third)]
            },

            // 吐: tu
            ['吐'] = new()
            {
                PinYins = [new("tu", CharTone.Third)]
            },

            // 吓: xia | he
            ['吓'] = new()
            {
                PinYins = [
                    new("xia", CharTone.Fourth),
                    new("he", CharTone.Fourth, cases: ["恐吓", "恫吓", "威吓"])
                ]
            },

            // 虫: chong
            ['虫'] = new()
            {
                PinYins = [new("chong", CharTone.Second)]
            },

            // 曲: qu
            ['曲'] = new()
            {
                PinYins = [
                    new("qu", CharTone.First, isFamilyName: true),
                    new("qu", CharTone.Third, cases: ["歌曲", "乐曲", "曲谱", "曲调", "曲艺", "异曲同工", "曲高和寡"])
                ]
            },

            // 团: tuan
            ['团'] = new()
            {
                PinYins = [new("tuan", CharTone.Second)]
            },

            // 同: tong
            ['同'] = new()
            {
                PinYins = [new("tong", CharTone.Second)]
            },

            // 吊: diao
            ['吊'] = new()
            {
                PinYins = [new("diao", CharTone.Fourth)]
            },

            // 吃: chi
            ['吃'] = new()
            {
                PinYins = [new("chi", CharTone.First)]
            },

            // 因: yin
            ['因'] = new()
            {
                PinYins = [new("yin", CharTone.First)]
            },

            // 吸: xi
            ['吸'] = new()
            {
                PinYins = [new("xi", CharTone.First)]
            },

            // 吗: ma
            ['吗'] = new()
            {
                PinYins = [new("ma", CharTone.First)]
            },

            // 屿: yu
            ['屿'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 帆: fan
            ['帆'] = new()
            {
                PinYins = [new("fan", CharTone.First)]
            },

            // 岁: sui
            ['岁'] = new()
            {
                PinYins = [new("sui", CharTone.Fourth)]
            },

            // 回: hui
            ['回'] = new()
            {
                PinYins = [new("hui", CharTone.Second)]
            },

            // 岂: qi
            ['岂'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 刚: gang
            ['刚'] = new()
            {
                PinYins = [new("gang", CharTone.First)]
            },

            // 则: ze
            ['则'] = new()
            {
                PinYins = [new("ze", CharTone.Second)]
            },

            // 肉: rou
            ['肉'] = new()
            {
                PinYins = [new("rou", CharTone.Fourth)]
            },

            // 网: wang
            ['网'] = new()
            {
                PinYins = [new("wang", CharTone.Third)]
            },

            // 年: nian
            ['年'] = new()
            {
                PinYins = [new("nian", CharTone.Second)]
            },

            // 朱: zhu
            ['朱'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 先: xian
            ['先'] = new()
            {
                PinYins = [new("xian", CharTone.First)]
            },

            // 丢: diu
            ['丢'] = new()
            {
                PinYins = [new("diu", CharTone.First)]
            },

            // 舌: she
            ['舌'] = new()
            {
                PinYins = [new("she", CharTone.Second)]
            },

            // 竹: zhu
            ['竹'] = new()
            {
                PinYins = [new("zhu", CharTone.Second)]
            },

            // 迁: qian
            ['迁'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 乔: qiao
            ['乔'] = new()
            {
                PinYins = [new("qiao", CharTone.Second)]
            },

            // 伟: wei
            ['伟'] = new()
            {
                PinYins = [new("wei", CharTone.Third)]
            },

            // 传: chuan | zhuan
            ['传'] = new()
            {
                PinYins = [
                    new("chuan", CharTone.Second),
                    new("zhuan", CharTone.Fourth, cases: ["经传", "左传", "小传", "自传", "传记", "传略", "传赞", "树碑立传", "水浒传", "传舍"])
                ]
            },

            // 乒: ping
            ['乒'] = new()
            {
                PinYins = [new("ping", CharTone.First)]
            },

            // 乓: pang
            ['乓'] = new()
            {
                PinYins = [new("pang", CharTone.First)]
            },

            // 休: xiu
            ['休'] = new()
            {
                PinYins = [new("xiu", CharTone.First)]
            },

            // 伍: wu
            ['伍'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 伏: fu
            ['伏'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 优: you
            ['优'] = new()
            {
                PinYins = [new("you", CharTone.First)]
            },

            // 伐: fa
            ['伐'] = new()
            {
                PinYins = [new("fa", CharTone.Second)]
            },

            // 延: yan
            ['延'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 件: jian
            ['件'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 任: ren
            ['任'] = new()
            {
                PinYins = [
                    new("ren", CharTone.Fourth),
                    new("ren", CharTone.Second, isFamilyName: true)
                ]
            },

            // 伤: shang
            ['伤'] = new()
            {
                PinYins = [new("shang", CharTone.First)]
            },

            // 价: jia
            ['价'] = new()
            {
                PinYins = [new("jia", CharTone.Fourth)]
            },

            // 份: fen
            ['份'] = new()
            {
                PinYins = [new("fen", CharTone.Fourth)]
            },

            // 华: hua
            ['华'] = new()
            {
                PinYins = [new("hua", CharTone.Second)]
            },

            // 仰: yang
            ['仰'] = new()
            {
                PinYins = [new("yang", CharTone.Third)]
            },

            // 仿: fang
            ['仿'] = new()
            {
                PinYins = [new("fang", CharTone.Third)]
            },

            // 伙: huo
            ['伙'] = new()
            {
                PinYins = [new("huo", CharTone.Third)]
            },

            // 伪: wei
            ['伪'] = new()
            {
                PinYins = [new("wei", CharTone.Third)]
            },

            // 自: zi
            ['自'] = new()
            {
                PinYins = [new("zi", CharTone.Fourth)]
            },

            // 血: xue | xie
            ['血'] = new()
            {
                PinYins = [
                    new("xue", CharTone.Fourth),
                    new("xie", CharTone.Third, cases: ["鸡血", "血块", "流血", "吐血"])
                ]
            },

            // 向: xiang
            ['向'] = new()
            {
                PinYins = [new("xiang", CharTone.Fourth)]
            },

            // 似: si | shi
            ['似'] = new()
            {
                PinYins = [
                    new("si", CharTone.Fourth),
                    new("shi", CharTone.Fourth, cases: ["似的"])
                ]
            },

            // 后: hou
            ['后'] = new()
            {
                PinYins = [new("hou", CharTone.Fourth)]
            },

            // 行: xing | hang
            ['行'] = new()
            {
                PinYins = [
                    new("xing", CharTone.Second, isFamilyName: true),
                    new("hang", CharTone.Second, cases: ["字里行间", "闵行", "行列", "罗列成行", "排行", "银行", "几行", "商行", "同行", "各行各业"])
                ]
            },

            // 舟: zhou
            ['舟'] = new()
            {
                PinYins = [new("zhou", CharTone.First)]
            },

            // 全: quan
            ['全'] = new()
            {
                PinYins = [new("quan", CharTone.Second)]
            },

            // 会: hui | kuai
            ['会'] = new()
            {
                PinYins = [
                    new("hui", CharTone.Fourth),
                    new("kuai", CharTone.Fourth, cases: ["会计", "会稽"])
                ]
            },

            // 杀: sha
            ['杀'] = new()
            {
                PinYins = [new("sha", CharTone.First)]
            },

            // 合: he
            ['合'] = new()
            {
                PinYins = [new("he", CharTone.Second)]
            },

            // 兆: zhao
            ['兆'] = new()
            {
                PinYins = [new("zhao", CharTone.Fourth)]
            },

            // 企: qi
            ['企'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 众: zhong
            ['众'] = new()
            {
                PinYins = [new("zhong", CharTone.Fourth)]
            },

            // 爷: ye
            ['爷'] = new()
            {
                PinYins = [new("ye", CharTone.Second)]
            },

            // 伞: san
            ['伞'] = new()
            {
                PinYins = [new("san", CharTone.Third)]
            },

            // 创: chuang
            ['创'] = new()
            {
                PinYins = [
                    new("chuang", CharTone.Fourth),
                    new("chuang", CharTone.First, cases: ["重创", "创伤", "创口", "创痕", "创面", "刃创", "千创", "重创"])
                ]
            },

            // 肌: ji
            ['肌'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 朵: duo
            ['朵'] = new()
            {
                PinYins = [new("duo", CharTone.Third)]
            },

            // 杂: za
            ['杂'] = new()
            {
                PinYins = [new("za", CharTone.Second)]
            },

            // 危: wei
            ['危'] = new()
            {
                PinYins = [new("wei", CharTone.First)]
            },

            // 旬: xun
            ['旬'] = new()
            {
                PinYins = [new("xun", CharTone.Second)]
            },

            // 旨: zhi
            ['旨'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 负: fu
            ['负'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 各: ge
            ['各'] = new()
            {
                PinYins = [new("ge", CharTone.Fourth)]
            },

            // 名: ming
            ['名'] = new()
            {
                PinYins = [new("ming", CharTone.Second)]
            },

            // 多: duo
            ['多'] = new()
            {
                PinYins = [new("duo", CharTone.First)]
            },

            // 争: zheng
            ['争'] = new()
            {
                PinYins = [new("zheng", CharTone.First)]
            },

            // 色: se | shai
            ['色'] = new()
            {
                PinYins = [
                    new("se", CharTone.Fourth),
                    new("shai", CharTone.Third, cases: ["色子"])
                ]
            },

            // 壮: zhuang
            ['壮'] = new()
            {
                PinYins = [new("zhuang", CharTone.Fourth)]
            },

            // 冲: chong
            ['冲'] = new()
            {
                PinYins = [
                    new("chong", CharTone.First),
                    new("chong", CharTone.Fourth, cases: ["冲床", "冲压", "冲模"])
                ]
            },

            // 冰: bing
            ['冰'] = new()
            {
                PinYins = [new("bing", CharTone.First)]
            },

            // 庄: zhuang
            ['庄'] = new()
            {
                PinYins = [new("zhuang", CharTone.First)]
            },

            // 庆: qing
            ['庆'] = new()
            {
                PinYins = [new("qing", CharTone.Fourth)]
            },

            // 亦: yi
            ['亦'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 刘: liu
            ['刘'] = new()
            {
                PinYins = [new("liu", CharTone.Second)]
            },

            // 齐: qi
            ['齐'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 交: jiao
            ['交'] = new()
            {
                PinYins = [new("jiao", CharTone.First)]
            },

            // 次: ci
            ['次'] = new()
            {
                PinYins = [new("ci", CharTone.Fourth)]
            },

            // 衣: yi
            ['衣'] = new()
            {
                PinYins = [new("yi", CharTone.First)]
            },

            // 产: chan
            ['产'] = new()
            {
                PinYins = [new("chan", CharTone.Third)]
            },

            // 决: jue
            ['决'] = new()
            {
                PinYins = [new("jue", CharTone.Second)]
            },

            // 充: chong
            ['充'] = new()
            {
                PinYins = [new("chong", CharTone.First)]
            },

            // 妄: wang
            ['妄'] = new()
            {
                PinYins = [new("wang", CharTone.Fourth)]
            },

            // 闭: bi
            ['闭'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 问: wen
            ['问'] = new()
            {
                PinYins = [new("wen", CharTone.Fourth)]
            },

            // 闯: chuang
            ['闯'] = new()
            {
                PinYins = [new("chuang", CharTone.Third)]
            },

            // 羊: yang
            ['羊'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 并: bing
            ['并'] = new()
            {
                PinYins = [new("bing", CharTone.Fourth)]
            },

            // 关: guan
            ['关'] = new()
            {
                PinYins = [new("guan", CharTone.First)]
            },

            // 米: mi
            ['米'] = new()
            {
                PinYins = [new("mi", CharTone.Third)]
            },

            // 灯: deng
            ['灯'] = new()
            {
                PinYins = [new("deng", CharTone.First)]
            },

            // 州: zhou
            ['州'] = new()
            {
                PinYins = [new("zhou", CharTone.First)]
            },

            // 汗: han
            ['汗'] = new()
            {
                PinYins = [new("han", CharTone.Fourth)]
            },

            // 污: wu
            ['污'] = new()
            {
                PinYins = [new("wu", CharTone.First)]
            },

            // 江: jiang
            ['江'] = new()
            {
                PinYins = [new("jiang", CharTone.First)]
            },

            // 池: chi
            ['池'] = new()
            {
                PinYins = [new("chi", CharTone.Second)]
            },

            // 汤: tang | shang
            ['汤'] = new()
            {
                PinYins = [
                    new("tang", CharTone.First),
                    new("shang", CharTone.Fourth, cases: ["汤汤"])
                ]
            },

            // 忙: mang
            ['忙'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 兴: xing
            ['兴'] = new()
            {
                PinYins = [new("xing", CharTone.First)]
            },

            // 宇: yu
            ['宇'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 守: shou
            ['守'] = new()
            {
                PinYins = [new("shou", CharTone.Third)]
            },

            // 宅: zhai
            ['宅'] = new()
            {
                PinYins = [new("zhai", CharTone.Second)]
            },

            // 字: zi
            ['字'] = new()
            {
                PinYins = [new("zi", CharTone.Fourth)]
            },

            // 安: an
            ['安'] = new()
            {
                PinYins = [new("an", CharTone.First)]
            },

            // 讲: jiang
            ['讲'] = new()
            {
                PinYins = [new("jiang", CharTone.Third)]
            },

            // 军: jun
            ['军'] = new()
            {
                PinYins = [new("jun", CharTone.First)]
            },

            // 许: xu
            ['许'] = new()
            {
                PinYins = [new("xu", CharTone.Third)]
            },

            // 论: lun
            ['论'] = new()
            {
                PinYins = [
                    new("lun", CharTone.Fourth),
                    new("lun", CharTone.Second, cases: ["论语"])
                ]
            },

            // 农: nong
            ['农'] = new()
            {
                PinYins = [new("nong", CharTone.Second)]
            },

            // 讽: feng
            ['讽'] = new()
            {
                PinYins = [new("feng", CharTone.Third)]
            },

            // 设: she
            ['设'] = new()
            {
                PinYins = [new("she", CharTone.Fourth)]
            },

            // 访: fang
            ['访'] = new()
            {
                PinYins = [new("fang", CharTone.Third)]
            },

            // 寻: xun
            ['寻'] = new()
            {
                PinYins = [new("xun", CharTone.Second)]
            },

            // 那: na | nei | na
            ['那'] = new()
            {
                PinYins = [
                    new("na", CharTone.Fourth),
                    new("nei", CharTone.Second),
                    new("na", CharTone.First, isFamilyName: true)
                ]
            },

            // 迅: xun
            ['迅'] = new()
            {
                PinYins = [new("xun", CharTone.Fourth)]
            },

            // 尽: jin
            ['尽'] = new()
            {
                PinYins = [new("jin", CharTone.Fourth)]
            }
        };
    }
}
