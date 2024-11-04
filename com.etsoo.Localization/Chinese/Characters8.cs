namespace com.etsoo.Localization.Chinese
{
    /// <summary>
    /// Chinese characters
    /// 中文字符
    /// </summary>
    internal static partial class ChineseCharacters
    {
        /// <summary>
        /// All Chinese characters 8 (second level)
        /// 所有中文字符8（二级汉字）
        /// </summary>
        public static Dictionary<char, ChineseCharacter> Characters8 => new(800)
        {
            // 乂: yi | ai
            ['乂'] = new()
            {
                PinYins = [
                    new("yi", CharTone.Fourth),
                    new("ai", CharTone.Fourth, cases: ["惩乂"])
                ]
            },

            // 乜: mie | nie
            ['乜'] = new()
            {
                PinYins = [
                    new("mie", CharTone.First),
                    new("nie", CharTone.Fourth, isFamilyName: true, cases: ["乜河"])
                ]
            },

            // 兀: wu
            ['兀'] = new()
            {
                PinYins = [
                    new("wu", CharTone.Fourth),
                    new("wu", CharTone.First, cases: ["兀秃"])
                ]
            },

            // 弋: yi
            ['弋'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 孑: jie
            ['孑'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 孓: jue
            ['孓'] = new()
            {
                PinYins = [new("jue", CharTone.Second)]
            },

            // 幺: yao
            ['幺'] = new()
            {
                PinYins = [new("yao", CharTone.First)]
            },

            // 亓: qi
            ['亓'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 韦: wei
            ['韦'] = new()
            {
                PinYins = [new("wei", CharTone.Second)]
            },

            // 廿: nian
            ['廿'] = new()
            {
                PinYins = [new("nian", CharTone.Fourth)]
            },

            // 丏: mian
            ['丏'] = new()
            {
                PinYins = [new("mian", CharTone.Third)]
            },

            // 卅: sa
            ['卅'] = new()
            {
                PinYins = [new("sa", CharTone.Fourth)]
            },

            // 仄: ze
            ['仄'] = new()
            {
                PinYins = [new("ze", CharTone.Fourth)]
            },

            // 厄: e
            ['厄'] = new()
            {
                PinYins = [new("e", CharTone.Fourth)]
            },

            // 仃: ding
            ['仃'] = new()
            {
                PinYins = [new("ding", CharTone.First)]
            },

            // 仉: zhang
            ['仉'] = new()
            {
                PinYins = [new("zhang", CharTone.Third)]
            },

            // 仂: le
            ['仂'] = new()
            {
                PinYins = [new("le", CharTone.Fourth)]
            },

            // 兮: xi
            ['兮'] = new()
            {
                PinYins = [new("xi", CharTone.First)]
            },

            // 刈: yi
            ['刈'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 爻: yao
            ['爻'] = new()
            {
                PinYins = [new("yao", CharTone.Second)]
            },

            // 卞: bian
            ['卞'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 闩: shuan
            ['闩'] = new()
            {
                PinYins = [new("shuan", CharTone.First)]
            },

            // 讣: fu
            ['讣'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 尹: yin
            ['尹'] = new()
            {
                PinYins = [new("yin", CharTone.Third)]
            },

            // 夬: guai
            ['夬'] = new()
            {
                PinYins = [new("guai", CharTone.Fourth)]
            },

            // 爿: pan
            ['爿'] = new()
            {
                PinYins = [new("pan", CharTone.Second)]
            },

            // 毋: wu
            ['毋'] = new()
            {
                PinYins = [new("wu", CharTone.Second)]
            },

            // 邗: han
            ['邗'] = new()
            {
                PinYins = [new("han", CharTone.Second)]
            },

            // 邛: qiong
            ['邛'] = new()
            {
                PinYins = [new("qiong", CharTone.Second)]
            },

            // 艽: jiao | qiu
            ['艽'] = new()
            {
                PinYins = [
                    new("jiao", CharTone.First),
                    new("qiu", CharTone.Second)
                ]
            },

            // 艿: nai
            ['艿'] = new()
            {
                PinYins = [new("nai", CharTone.Third)]
            },

            // 札: zha
            ['札'] = new()
            {
                PinYins = [new("zha", CharTone.Second)]
            },

            // 叵: po
            ['叵'] = new()
            {
                PinYins = [new("po", CharTone.Third)]
            },

            // 匝: za
            ['匝'] = new()
            {
                PinYins = [new("za", CharTone.First)]
            },

            // 丕: pi
            ['丕'] = new()
            {
                PinYins = [new("pi", CharTone.First)]
            },

            // 匜: yi
            ['匜'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 劢: mai
            ['劢'] = new()
            {
                PinYins = [new("mai", CharTone.Fourth)]
            },

            // 卟: bu
            ['卟'] = new()
            {
                PinYins = [new("bu", CharTone.Third)]
            },

            // 叱: chi
            ['叱'] = new()
            {
                PinYins = [new("chi", CharTone.Fourth)]
            },

            // 叻: le
            ['叻'] = new()
            {
                PinYins = [new("le", CharTone.Fourth)]
            },

            // 仨: sa
            ['仨'] = new()
            {
                PinYins = [new("sa", CharTone.First)]
            },

            // 仕: shi
            ['仕'] = new()
            {
                PinYins = [new("shi", CharTone.Fourth)]
            },

            // 仟: qian
            ['仟'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 仡: ge | yi
            ['仡'] = new()
            {
                PinYins = [
                    new("yi", CharTone.Fourth),
                    new("ge", CharTone.First, cases: ["仡佬"])
                ]
            },

            // 仫: mu
            ['仫'] = new()
            {
                PinYins = [new("mu", CharTone.Fourth)]
            },

            // 仞: ren
            ['仞'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 卮: zhi
            ['卮'] = new()
            {
                PinYins = [new("zhi", CharTone.First)]
            },

            // 氐: di
            ['氐'] = new()
            {
                PinYins = [new("di", CharTone.First)]
            },

            // 犰: qiu
            ['犰'] = new()
            {
                PinYins = [new("qiu", CharTone.Second)]
            },

            // 刍: chu
            ['刍'] = new()
            {
                PinYins = [new("chu", CharTone.Second)]
            },

            // 邝: kuang
            ['邝'] = new()
            {
                PinYins = [new("kuang", CharTone.Fourth)]
            },

            // 邙: mang
            ['邙'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 汀: ting
            ['汀'] = new()
            {
                PinYins = [new("ting", CharTone.First)]
            },

            // 讦: jie
            ['讦'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 讧: hong
            ['讧'] = new()
            {
                PinYins = [new("hong", CharTone.Fourth)]
            },

            // 讪: shan
            ['讪'] = new()
            {
                PinYins = [new("shan", CharTone.Fourth)]
            },

            // 讫: qi
            ['讫'] = new()
            {
                PinYins = [new("qi", CharTone.Fourth)]
            },

            // 尻: kao
            ['尻'] = new()
            {
                PinYins = [new("kao", CharTone.First)]
            },

            // 阡: qian
            ['阡'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 尕: ga
            ['尕'] = new()
            {
                PinYins = [new("ga", CharTone.Third)]
            },

            // 弁: bian
            ['弁'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 驭: yu
            ['驭'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 匡: kuang
            ['匡'] = new()
            {
                PinYins = [new("kuang", CharTone.First)]
            },

            // 耒: lei
            ['耒'] = new()
            {
                PinYins = [new("lei", CharTone.Third)]
            },

            // 玎: ding
            ['玎'] = new()
            {
                PinYins = [new("ding", CharTone.First)]
            },

            // 玑: ji
            ['玑'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 邢: xing
            ['邢'] = new()
            {
                PinYins = [new("xing", CharTone.Second)]
            },

            // 圩: wei | xu | yu
            ['圩'] = new()
            {
                PinYins = [
                    new("wei", CharTone.Second),
                    new("xu", CharTone.First, cases: ["圩场", "圩日", "圩期", "圩市", "圩镇", "圩一", "圩二", "圩三"]),
                    new("yu", CharTone.Second, cases: ["圩顶"])
                ]
            },

            // 圬: wu
            ['圬'] = new()
            {
                PinYins = [new("wu", CharTone.First)]
            },

            // 圭: gui
            ['圭'] = new()
            {
                PinYins = [new("gui", CharTone.First)]
            },

            // 扦: qian
            ['扦'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 圪: ge
            ['圪'] = new()
            {
                PinYins = [new("ge", CharTone.First)]
            },

            // 圳: zhen
            ['圳'] = new()
            {
                PinYins = [new("zhen", CharTone.Fourth)]
            },

            // 圹: kuang
            ['圹'] = new()
            {
                PinYins = [new("kuang", CharTone.Fourth)]
            },

            // 扪: men
            ['扪'] = new()
            {
                PinYins = [new("men", CharTone.Second)]
            },

            // 圮: pi
            ['圮'] = new()
            {
                PinYins = [new("pi", CharTone.Third)]
            },

            // 圯: yi
            ['圯'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 芊: qian
            ['芊'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 芍: shao
            ['芍'] = new()
            {
                PinYins = [new("shao", CharTone.Second)]
            },

            // 芄: wan
            ['芄'] = new()
            {
                PinYins = [new("wan", CharTone.Second)]
            },

            // 芨: ji
            ['芨'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 芑: qi
            ['芑'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 芎: xiong
            ['芎'] = new()
            {
                PinYins = [new("xiong", CharTone.First)]
            },

            // 芗: xiang
            ['芗'] = new()
            {
                PinYins = [new("xiang", CharTone.First)]
            },

            // 亘: gen
            ['亘'] = new()
            {
                PinYins = [new("gen", CharTone.Fourth)]
            },

            // 厍: she
            ['厍'] = new()
            {
                PinYins = [new("she", CharTone.Fourth)]
            },

            // 夼: kuang
            ['夼'] = new()
            {
                PinYins = [new("kuang", CharTone.Third)]
            },

            // 戍: shu
            ['戍'] = new()
            {
                PinYins = [new("shu", CharTone.Fourth)]
            },

            // 尥: liao
            ['尥'] = new()
            {
                PinYins = [new("liao", CharTone.Fourth)]
            },

            // 乩: ji
            ['乩'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 旯: la
            ['旯'] = new()
            {
                PinYins = [new("la", CharTone.Second)]
            },

            // 曳: ye
            ['曳'] = new()
            {
                PinYins = [new("ye", CharTone.Fourth)]
            },

            // 岌: ji
            ['岌'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 屺: qi
            ['屺'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 凼: dang
            ['凼'] = new()
            {
                PinYins = [new("dang", CharTone.Fourth)]
            },

            // 囡: nan
            ['囡'] = new()
            {
                PinYins = [new("nan", CharTone.First)]
            },

            // 钇: yi
            ['钇'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 缶: fou
            ['缶'] = new()
            {
                PinYins = [new("fou", CharTone.Third)]
            },

            // 氘: dao
            ['氘'] = new()
            {
                PinYins = [new("dao", CharTone.First)]
            },

            // 氖: nai
            ['氖'] = new()
            {
                PinYins = [new("nai", CharTone.Third)]
            },

            // 牝: pin
            ['牝'] = new()
            {
                PinYins = [new("pin", CharTone.Fourth)]
            },

            // 伎: ji
            ['伎'] = new()
            {
                PinYins = [new("ji", CharTone.Fourth)]
            },

            // 伛: yu
            ['伛'] = new()
            {
                PinYins = [new("yu", CharTone.Third)]
            },

            // 伢: ya
            ['伢'] = new()
            {
                PinYins = [new("ya", CharTone.Second)]
            },

            // 佤: wa
            ['佤'] = new()
            {
                PinYins = [new("wa", CharTone.Third)]
            },

            // 仵: wu
            ['仵'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 伥: chang
            ['伥'] = new()
            {
                PinYins = [new("chang", CharTone.First)]
            },

            // 伧: cang
            ['伧'] = new()
            {
                PinYins = [new("cang", CharTone.First)]
            },

            // 伉: kang
            ['伉'] = new()
            {
                PinYins = [new("kang", CharTone.Fourth)]
            },

            // 伫: zhu
            ['伫'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 囟: xin
            ['囟'] = new()
            {
                PinYins = [new("xin", CharTone.Fourth)]
            },

            // 汆: cuan
            ['汆'] = new()
            {
                PinYins = [new("cuan", CharTone.First)]
            },

            // 刖: yue
            ['刖'] = new()
            {
                PinYins = [new("yue", CharTone.Fourth)]
            },

            // 夙: su
            ['夙'] = new()
            {
                PinYins = [new("su", CharTone.Fourth)]
            },

            // 旮: ga
            ['旮'] = new()
            {
                PinYins = [new("ga", CharTone.First)]
            },

            // 刎: wen
            ['刎'] = new()
            {
                PinYins = [new("wen", CharTone.Third)]
            },

            // 犷: guang
            ['犷'] = new()
            {
                PinYins = [new("guang", CharTone.Third)]
            },

            // 犸: ma
            ['犸'] = new()
            {
                PinYins = [new("ma", CharTone.Third)]
            },

            // 舛: chuan
            ['舛'] = new()
            {
                PinYins = [new("chuan", CharTone.Third)]
            },

            // 凫: fu
            ['凫'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 邬: wu
            ['邬'] = new()
            {
                PinYins = [new("wu", CharTone.First)]
            },

            // 饧: tang | xing
            ['饧'] = new()
            {
                PinYins = [
                    new("tang", CharTone.Second),
                    new("xing", CharTone.Second)
                ]
            },

            // 汕: shan
            ['汕'] = new()
            {
                PinYins = [new("shan", CharTone.Fourth)]
            },

            // 汔: qi
            ['汔'] = new()
            {
                PinYins = [new("qi", CharTone.Fourth)]
            },

            // 汐: xi
            ['汐'] = new()
            {
                PinYins = [new("xi", CharTone.First)]
            },

            // 汲: ji
            ['汲'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 汜: si
            ['汜'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 汊: cha
            ['汊'] = new()
            {
                PinYins = [new("cha", CharTone.Fourth)]
            },

            // 忖: cun
            ['忖'] = new()
            {
                PinYins = [new("cun", CharTone.Third)]
            },

            // 忏: chan
            ['忏'] = new()
            {
                PinYins = [new("chan", CharTone.Fourth)]
            },

            // 讴: ou
            ['讴'] = new()
            {
                PinYins = [new("ou", CharTone.First)]
            },

            // 讵: ju
            ['讵'] = new()
            {
                PinYins = [new("ju", CharTone.Fourth)]
            },

            // 祁: qi
            ['祁'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 讷: ne
            ['讷'] = new()
            {
                PinYins = [new("ne", CharTone.Fourth)]
            },

            // 聿: yu
            ['聿'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 艮: gen
            ['艮'] = new()
            {
                PinYins = [new("gen", CharTone.Fourth)]
            },

            // 厾: du
            ['厾'] = new()
            {
                PinYins = [new("du", CharTone.First)]
            },

            // 阱: jing
            ['阱'] = new()
            {
                PinYins = [new("jing", CharTone.Third)]
            },

            // 阮: ruan
            ['阮'] = new()
            {
                PinYins = [new("ruan", CharTone.Third)]
            },

            // 阪: ban
            ['阪'] = new()
            {
                PinYins = [new("ban", CharTone.Third)]
            },

            // 丞: cheng
            ['丞'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 妁: shuo
            ['妁'] = new()
            {
                PinYins = [new("shuo", CharTone.Fourth)]
            },

            // 牟: mou
            ['牟'] = new()
            {
                PinYins = [new("mou", CharTone.Second)]
            },

            // 纡: yu
            ['纡'] = new()
            {
                PinYins = [new("yu", CharTone.First)]
            },

            // 纣: zhou
            ['纣'] = new()
            {
                PinYins = [new("zhou", CharTone.Fourth)]
            },

            // 纥: he
            ['纥'] = new()
            {
                PinYins = [new("he", CharTone.Second)]
            },

            // 纨: wan
            ['纨'] = new()
            {
                PinYins = [new("wan", CharTone.Second)]
            },

            // 玕: gan
            ['玕'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 玙: yu
            ['玙'] = new()
            {
                PinYins = [new("yu", CharTone.Second)]
            },

            // 抟: tuan
            ['抟'] = new()
            {
                PinYins = [new("tuan", CharTone.Second)]
            },

            // 抔: pou
            ['抔'] = new()
            {
                PinYins = [new("pou", CharTone.Second)]
            },

            // 圻: qi
            ['圻'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 坂: ban
            ['坂'] = new()
            {
                PinYins = [new("ban", CharTone.Third)]
            },

            // 坍: tan
            ['坍'] = new()
            {
                PinYins = [new("tan", CharTone.First)]
            },

            // 坞: wu
            ['坞'] = new()
            {
                PinYins = [new("wu", CharTone.Fourth)]
            },

            // 抃: bian
            ['抃'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 抉: jue
            ['抉'] = new()
            {
                PinYins = [new("jue", CharTone.Second)]
            },

            // 㧐: zhuo
            ['㧐'] = new()
            {
                PinYins = [new("zhuo", CharTone.Second)]
            },

            // 芫: yuan
            ['芫'] = new()
            {
                PinYins = [new("yuan", CharTone.Second)]
            },

            // 邯: han
            ['邯'] = new()
            {
                PinYins = [new("han", CharTone.Second)]
            },

            // 芸: yun
            ['芸'] = new()
            {
                PinYins = [new("yun", CharTone.Second)]
            },

            // 芾: fei
            ['芾'] = new()
            {
                PinYins = [new("fei", CharTone.First)]
            },

            // 苈: li
            ['苈'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 苣: ju
            ['苣'] = new()
            {
                PinYins = [new("ju", CharTone.Fourth)]
            },

            // 芷: zhi
            ['芷'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 芮: rui
            ['芮'] = new()
            {
                PinYins = [new("rui", CharTone.Fourth)]
            },

            // 苋: xian
            ['苋'] = new()
            {
                PinYins = [new("xian", CharTone.Fourth)]
            },

            // 芼: mao
            ['芼'] = new()
            {
                PinYins = [new("mao", CharTone.Fourth)]
            },

            // 苌: chang
            ['苌'] = new()
            {
                PinYins = [new("chang", CharTone.Second)]
            },

            // 苁: cong
            ['苁'] = new()
            {
                PinYins = [new("cong", CharTone.First)]
            },

            // 芩: qin
            ['芩'] = new()
            {
                PinYins = [new("qin", CharTone.Second)]
            },

            // 芪: qi
            ['芪'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 芡: qian
            ['芡'] = new()
            {
                PinYins = [new("qian", CharTone.Fourth)]
            },

            // 芟: shan
            ['芟'] = new()
            {
                PinYins = [new("shan", CharTone.First)]
            },

            // 苄: bian
            ['苄'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 苎: zhu
            ['苎'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 苡: yi
            ['苡'] = new()
            {
                PinYins = [new("yi", CharTone.Third)]
            },

            // 杌: wu
            ['杌'] = new()
            {
                PinYins = [new("wu", CharTone.Fourth)]
            },

            // 杓: shao
            ['杓'] = new()
            {
                PinYins = [new("shao", CharTone.Second)]
            },

            // 杞: qi
            ['杞'] = new()
            {
                PinYins = [new("qi", CharTone.Third)]
            },

            // 杈: cha
            ['杈'] = new()
            {
                PinYins = [new("cha", CharTone.First)]
            },

            // 忑: te
            ['忑'] = new()
            {
                PinYins = [new("te", CharTone.Fourth)]
            },

            // 孛: bei
            ['孛'] = new()
            {
                PinYins = [new("bei", CharTone.Fourth)]
            },

            // 邴: bing
            ['邴'] = new()
            {
                PinYins = [new("bing", CharTone.Third)]
            },

            // 邳: pi
            ['邳'] = new()
            {
                PinYins = [new("pi", CharTone.First)]
            },

            // 矶: ji
            ['矶'] = new()
            {
                PinYins = [new("ji", CharTone.First)]
            },

            // 奁: lian
            ['奁'] = new()
            {
                PinYins = [new("lian", CharTone.Second)]
            },

            // 豕: shi
            ['豕'] = new()
            {
                PinYins = [new("shi", CharTone.Third)]
            },

            // 忒: te
            ['忒'] = new()
            {
                PinYins = [new("te", CharTone.Fourth)]
            },

            // 欤: yu
            ['欤'] = new()
            {
                PinYins = [new("yu", CharTone.Second)]
            },

            // 轫: ren
            ['轫'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 迓: ya
            ['迓'] = new()
            {
                PinYins = [new("ya", CharTone.Fourth)]
            },

            // 邶: bei
            ['邶'] = new()
            {
                PinYins = [new("bei", CharTone.Fourth)]
            },

            // 忐: tan
            ['忐'] = new()
            {
                PinYins = [new("tan", CharTone.Third)]
            },

            // 卣: you
            ['卣'] = new()
            {
                PinYins = [new("you", CharTone.Third)]
            },

            // 邺: ye
            ['邺'] = new()
            {
                PinYins = [new("ye", CharTone.Fourth)]
            },

            // 旰: gan
            ['旰'] = new()
            {
                PinYins = [new("gan", CharTone.Fourth)]
            },

            // 呋: fu
            ['呋'] = new()
            {
                PinYins = [new("fu", CharTone.First)]
            },

            // 呒: mu
            ['呒'] = new()
            {
                PinYins = [new("mu", CharTone.Third)]
            },

            // 呓: yi
            ['呓'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 呔: dai
            ['呔'] = new()
            {
                PinYins = [new("dai", CharTone.First)]
            },

            // 呖: li
            ['呖'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 呃: e
            ['呃'] = new()
            {
                PinYins = [new("e", CharTone.Fourth)]
            },

            // 旸: yang
            ['旸'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 吡: bi
            ['吡'] = new()
            {
                PinYins = [new("bi", CharTone.Third)]
            },

            // 町: ting
            ['町'] = new()
            {
                PinYins = [new("ting", CharTone.Third)]
            },

            // 虬: qiu
            ['虬'] = new()
            {
                PinYins = [new("qiu", CharTone.Second)]
            },

            // 呗: bei
            ['呗'] = new()
            {
                PinYins = [new("bei", CharTone.Fourth)]
            },

            // 吽: hong
            ['吽'] = new()
            {
                PinYins = [new("hong", CharTone.First)]
            },

            // 吣: qin
            ['吣'] = new()
            {
                PinYins = [new("qin", CharTone.Fourth)]
            },

            // 吲: yin
            ['吲'] = new()
            {
                PinYins = [new("yin", CharTone.Third)]
            },

            // 帏: wei
            ['帏'] = new()
            {
                PinYins = [new("wei", CharTone.Second)]
            },

            // 岐: qi
            ['岐'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 岈: ya
            ['岈'] = new()
            {
                PinYins = [new("ya", CharTone.Second)]
            },

            // 岘: xian
            ['岘'] = new()
            {
                PinYins = [new("xian", CharTone.Fourth)]
            },

            // 岑: cen
            ['岑'] = new()
            {
                PinYins = [new("cen", CharTone.Second)]
            },

            // 岚: lan
            ['岚'] = new()
            {
                PinYins = [new("lan", CharTone.Second)]
            },

            // 兕: si
            ['兕'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 囵: lun
            ['囵'] = new()
            {
                PinYins = [new("lun", CharTone.Second)]
            },

            // 囫: hu
            ['囫'] = new()
            {
                PinYins = [new("hu", CharTone.Second)]
            },

            // 钊: zhao
            ['钊'] = new()
            {
                PinYins = [new("zhao", CharTone.First)]
            },

            // 钋: po
            ['钋'] = new()
            {
                PinYins = [new("po", CharTone.First)]
            },

            // 钌: liao
            ['钌'] = new()
            {
                PinYins = [new("liao", CharTone.Third)]
            },

            // 迕: wu
            ['迕'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 氙: xian
            ['氙'] = new()
            {
                PinYins = [new("xian", CharTone.First)]
            },

            // 氚: chuan
            ['氚'] = new()
            {
                PinYins = [new("chuan", CharTone.First)]
            },

            // 牤: mang
            ['牤'] = new()
            {
                PinYins = [new("mang", CharTone.Second)]
            },

            // 佞: ning
            ['佞'] = new()
            {
                PinYins = [new("ning", CharTone.Fourth)]
            },

            // 邱: qiu
            ['邱'] = new()
            {
                PinYins = [new("qiu", CharTone.First)]
            },

            // 攸: you
            ['攸'] = new()
            {
                PinYins = [new("you", CharTone.First)]
            },

            // 佚: yi
            ['佚'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 佝: gou
            ['佝'] = new()
            {
                PinYins = [new("gou", CharTone.First)]
            },

            // 佟: tong
            ['佟'] = new()
            {
                PinYins = [new("tong", CharTone.Second)]
            },

            // 佗: tuo
            ['佗'] = new()
            {
                PinYins = [new("tuo", CharTone.Second)]
            },

            // 伽: jia
            ['伽'] = new()
            {
                PinYins = [new("jia", CharTone.First)]
            },

            // 彷: pang
            ['彷'] = new()
            {
                PinYins = [new("pang", CharTone.Second)]
            },

            // 佘: she
            ['佘'] = new()
            {
                PinYins = [new("she", CharTone.Second)]
            },

            // 佥: qian
            ['佥'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 孚: fu
            ['孚'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 豸: zhi
            ['豸'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 坌: ben
            ['坌'] = new()
            {
                PinYins = [new("ben", CharTone.Fourth)]
            },

            // 肟: wo
            ['肟'] = new()
            {
                PinYins = [new("wo", CharTone.Fourth)]
            },

            // 邸: di
            ['邸'] = new()
            {
                PinYins = [new("di", CharTone.Third)]
            },

            // 奂: huan
            ['奂'] = new()
            {
                PinYins = [new("huan", CharTone.Fourth)]
            },

            // 劬: qu
            ['劬'] = new()
            {
                PinYins = [new("qu", CharTone.Second)]
            },

            // 狄: di
            ['狄'] = new()
            {
                PinYins = [new("di", CharTone.Second)]
            },

            // 狁: yun
            ['狁'] = new()
            {
                PinYins = [new("yun", CharTone.Third)]
            },

            // 鸠: jiu
            ['鸠'] = new()
            {
                PinYins = [new("jiu", CharTone.First)]
            },

            // 邹: zou
            ['邹'] = new()
            {
                PinYins = [new("zou", CharTone.First)]
            },

            // 饨: tun
            ['饨'] = new()
            {
                PinYins = [new("tun", CharTone.Second)]
            },

            // 饩: xi
            ['饩'] = new()
            {
                PinYins = [new("xi", CharTone.Fourth)]
            },

            // 饪: ren
            ['饪'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 饫: yu
            ['饫'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 饬: chi
            ['饬'] = new()
            {
                PinYins = [new("chi", CharTone.Fourth)]
            },

            // 亨: heng
            ['亨'] = new()
            {
                PinYins = [new("heng", CharTone.First)]
            },

            // 庑: wu
            ['庑'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 庋: xi
            ['庋'] = new()
            {
                PinYins = [new("xi", CharTone.Third)]
            },

            // 疔: ding
            ['疔'] = new()
            {
                PinYins = [new("ding", CharTone.First)]
            },

            // 疖: jie
            ['疖'] = new()
            {
                PinYins = [new("jie", CharTone.First)]
            },

            // 肓: huang
            ['肓'] = new()
            {
                PinYins = [new("huang", CharTone.First)]
            },

            // 闱: wei
            ['闱'] = new()
            {
                PinYins = [new("wei", CharTone.Second)]
            },

            // 闳: hong
            ['闳'] = new()
            {
                PinYins = [new("hong", CharTone.Second)]
            },

            // 闵: min
            ['闵'] = new()
            {
                PinYins = [new("min", CharTone.Third)]
            },

            // 羌: qiang
            ['羌'] = new()
            {
                PinYins = [new("qiang", CharTone.First)]
            },

            // 炀: yang
            ['炀'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 沣: feng
            ['沣'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 沅: yuan
            ['沅'] = new()
            {
                PinYins = [new("yuan", CharTone.Second)]
            },

            // 沔: mian
            ['沔'] = new()
            {
                PinYins = [new("mian", CharTone.Third)]
            },

            // 沤: ou
            ['沤'] = new()
            {
                PinYins = [new("ou", CharTone.Fourth)]
            },

            // 沌: dun
            ['沌'] = new()
            {
                PinYins = [new("dun", CharTone.Fourth)]
            },

            // 沏: qi
            ['沏'] = new()
            {
                PinYins = [new("qi", CharTone.First)]
            },

            // 沚: zhi
            ['沚'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 汩: gu
            ['汩'] = new()
            {
                PinYins = [new("gu", CharTone.Third)]
            },

            // 汨: mi
            ['汨'] = new()
            {
                PinYins = [new("mi", CharTone.Fourth)]
            },

            // 沂: yi
            ['沂'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 汾: fen
            ['汾'] = new()
            {
                PinYins = [new("fen", CharTone.Second)]
            },

            // 沨: feng
            ['沨'] = new()
            {
                PinYins = [new("feng", CharTone.First)]
            },

            // 汴: bian
            ['汴'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 汶: wen
            ['汶'] = new()
            {
                PinYins = [new("wen", CharTone.Second)]
            },

            // 沆: hang
            ['沆'] = new()
            {
                PinYins = [new("hang", CharTone.Fourth)]
            },

            // 沩: wei
            ['沩'] = new()
            {
                PinYins = [new("wei", CharTone.Second)]
            },

            // 泐: le
            ['泐'] = new()
            {
                PinYins = [new("le", CharTone.Fourth)]
            },

            // 怃: wu
            ['怃'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 怄: ou
            ['怄'] = new()
            {
                PinYins = [new("ou", CharTone.Fourth)]
            },

            // 忡: chong
            ['忡'] = new()
            {
                PinYins = [new("chong", CharTone.First)]
            },

            // 忤: wu
            ['忤'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 忾: kai
            ['忾'] = new()
            {
                PinYins = [new("kai", CharTone.Fourth)]
            },

            // 怅: chang
            ['怅'] = new()
            {
                PinYins = [new("chang", CharTone.Fourth)]
            },

            // 忻: xin
            ['忻'] = new()
            {
                PinYins = [new("xin", CharTone.First)]
            },

            // 忪: song
            ['忪'] = new()
            {
                PinYins = [new("song", CharTone.First)]
            },

            // 怆: chuang
            ['怆'] = new()
            {
                PinYins = [new("chuang", CharTone.Fourth)]
            },

            // 忭: bian
            ['忭'] = new()
            {
                PinYins = [new("bian", CharTone.Fourth)]
            },

            // 忸: niu
            ['忸'] = new()
            {
                PinYins = [new("niu", CharTone.Third)]
            },

            // 诂: gu
            ['诂'] = new()
            {
                PinYins = [new("gu", CharTone.Third)]
            },

            // 诃: he
            ['诃'] = new()
            {
                PinYins = [new("he", CharTone.First)]
            },

            // 诅: zu
            ['诅'] = new()
            {
                PinYins = [new("zu", CharTone.Third)]
            },

            // 诋: di
            ['诋'] = new()
            {
                PinYins = [new("di", CharTone.Third)]
            },

            // 诌: zhou
            ['诌'] = new()
            {
                PinYins = [new("zhou", CharTone.First)]
            },

            // 诏: zhao
            ['诏'] = new()
            {
                PinYins = [new("zhao", CharTone.Fourth)]
            },

            // 诒: yi
            ['诒'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 孜: zi
            ['孜'] = new()
            {
                PinYins = [new("zi", CharTone.First)]
            },

            // 陇: long
            ['陇'] = new()
            {
                PinYins = [new("long", CharTone.Third)]
            },

            // 陀: tuo
            ['陀'] = new()
            {
                PinYins = [new("tuo", CharTone.Second)]
            },

            // 陂: bei
            ['陂'] = new()
            {
                PinYins = [new("bei", CharTone.First)]
            },

            // 陉: xing
            ['陉'] = new()
            {
                PinYins = [new("xing", CharTone.Second)]
            },

            // 妍: yan
            ['妍'] = new()
            {
                PinYins = [new("yan", CharTone.Second)]
            },

            // 妩: wu
            ['妩'] = new()
            {
                PinYins = [new("wu", CharTone.Third)]
            },

            // 妪: yu
            ['妪'] = new()
            {
                PinYins = [new("yu", CharTone.Fourth)]
            },

            // 妣: bi
            ['妣'] = new()
            {
                PinYins = [new("bi", CharTone.Third)]
            },

            // 妊: ren
            ['妊'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 妗: jin
            ['妗'] = new()
            {
                PinYins = [new("jin", CharTone.Fourth)]
            },

            // 妫: gui
            ['妫'] = new()
            {
                PinYins = [new("gui", CharTone.First)]
            },

            // 妞: niu
            ['妞'] = new()
            {
                PinYins = [new("niu", CharTone.First)]
            },

            // 姒: si
            ['姒'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 妤: yu
            ['妤'] = new()
            {
                PinYins = [new("yu", CharTone.Second)]
            },

            // 邵: shao
            ['邵'] = new()
            {
                PinYins = [new("shao", CharTone.Fourth)]
            },

            // 劭: shao
            ['劭'] = new()
            {
                PinYins = [new("shao", CharTone.Fourth)]
            },

            // 刭: jing
            ['刭'] = new()
            {
                PinYins = [new("jing", CharTone.Third)]
            },

            // 甬: yong
            ['甬'] = new()
            {
                PinYins = [new("yong", CharTone.Third)]
            },

            // 邰: tai
            ['邰'] = new()
            {
                PinYins = [new("tai", CharTone.Second)]
            },

            // 纭: yun
            ['纭'] = new()
            {
                PinYins = [new("yun", CharTone.Second)]
            },

            // 纰: pi
            ['纰'] = new()
            {
                PinYins = [new("pi", CharTone.First)]
            },

            // 纴: ren
            ['纴'] = new()
            {
                PinYins = [new("ren", CharTone.Fourth)]
            },

            // 纶: lun
            ['纶'] = new()
            {
                PinYins = [new("lun", CharTone.Second)]
            },

            // 纾: shu
            ['纾'] = new()
            {
                PinYins = [new("shu", CharTone.First)]
            },

            // 玮: wei
            ['玮'] = new()
            {
                PinYins = [new("wei", CharTone.Third)]
            },

            // 玡: ya
            ['玡'] = new()
            {
                PinYins = [new("ya", CharTone.Second)]
            },

            // 玭: pin
            ['玭'] = new()
            {
                PinYins = [new("pin", CharTone.Second)]
            },

            // 玠: jie
            ['玠'] = new()
            {
                PinYins = [new("jie", CharTone.Fourth)]
            },

            // 玢: bin
            ['玢'] = new()
            {
                PinYins = [new("bin", CharTone.First)]
            },

            // 玥: yue
            ['玥'] = new()
            {
                PinYins = [new("yue", CharTone.Fourth)]
            },

            // 玦: jue
            ['玦'] = new()
            {
                PinYins = [new("jue", CharTone.Second)]
            },

            // 盂: yu
            ['盂'] = new()
            {
                PinYins = [new("yu", CharTone.Second)]
            },

            // 忝: tian
            ['忝'] = new()
            {
                PinYins = [new("tian", CharTone.Third)]
            },

            // 匦: gui
            ['匦'] = new()
            {
                PinYins = [new("gui", CharTone.Third)]
            },

            // 坩: gan
            ['坩'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 抨: peng
            ['抨'] = new()
            {
                PinYins = [new("peng", CharTone.First)]
            },

            // 拤: qia
            ['拤'] = new()
            {
                PinYins = [new("qia", CharTone.Second)]
            },

            // 坫: dian
            ['坫'] = new()
            {
                PinYins = [new("dian", CharTone.Fourth)]
            },

            // 拈: nian
            ['拈'] = new()
            {
                PinYins = [new("nian", CharTone.First)]
            },

            // 垆: lu
            ['垆'] = new()
            {
                PinYins = [new("lu", CharTone.Second)]
            },

            // 抻: chen
            ['抻'] = new()
            {
                PinYins = [new("chen", CharTone.First)]
            },

            // 劼: jie
            ['劼'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 拃: zha
            ['拃'] = new()
            {
                PinYins = [new("zha", CharTone.Third)]
            },

            // 拊: fu
            ['拊'] = new()
            {
                PinYins = [new("fu", CharTone.Third)]
            },

            // 坼: che
            ['坼'] = new()
            {
                PinYins = [new("che", CharTone.Fourth)]
            },

            // 坻: chi
            ['坻'] = new()
            {
                PinYins = [new("chi", CharTone.Second)]
            },

            // 㧟: ju
            ['㧟'] = new()
            {
                PinYins = [new("ju", CharTone.Third)]
            },

            // 坨: tuo
            ['坨'] = new()
            {
                PinYins = [new("tuo", CharTone.Second)]
            },

            // 坭: ni
            ['坭'] = new()
            {
                PinYins = [new("ni", CharTone.Second)]
            },

            // 抿: min
            ['抿'] = new()
            {
                PinYins = [new("min", CharTone.Third)]
            },

            // 坳: ao
            ['坳'] = new()
            {
                PinYins = [new("ao", CharTone.Fourth)]
            },

            // 耶: ye
            ['耶'] = new()
            {
                PinYins = [new("ye", CharTone.Second)]
            },

            // 苷: gan
            ['苷'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 苯: ben
            ['苯'] = new()
            {
                PinYins = [new("ben", CharTone.Third)]
            },

            // 苤: pie
            ['苤'] = new()
            {
                PinYins = [new("pie", CharTone.Third)]
            },

            // 茏: long
            ['茏'] = new()
            {
                PinYins = [new("long", CharTone.Second)]
            },

            // 苫: shan
            ['苫'] = new()
            {
                PinYins = [new("shan", CharTone.First)]
            },

            // 苜: mu
            ['苜'] = new()
            {
                PinYins = [new("mu", CharTone.Fourth)]
            },

            // 苴: ju
            ['苴'] = new()
            {
                PinYins = [new("ju", CharTone.First)]
            },

            // 苒: ran
            ['苒'] = new()
            {
                PinYins = [new("ran", CharTone.Third)]
            },

            // 苘: qing
            ['苘'] = new()
            {
                PinYins = [new("qing", CharTone.Third)]
            },

            // 茌: chi
            ['茌'] = new()
            {
                PinYins = [new("chi", CharTone.Second)]
            },

            // 苻: fu
            ['苻'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 苓: ling
            ['苓'] = new()
            {
                PinYins = [new("ling", CharTone.Second)]
            },

            // 茚: yin
            ['茚'] = new()
            {
                PinYins = [new("yin", CharTone.Fourth)]
            },

            // 茆: mao
            ['茆'] = new()
            {
                PinYins = [new("mao", CharTone.Second)]
            },

            // 茑: niao
            ['茑'] = new()
            {
                PinYins = [new("niao", CharTone.Third)]
            },

            // 茓: xue
            ['茓'] = new()
            {
                PinYins = [new("xue", CharTone.Second)]
            },

            // 茔: ying
            ['茔'] = new()
            {
                PinYins = [new("ying", CharTone.Second)]
            },

            // 茕: qiong
            ['茕'] = new()
            {
                PinYins = [new("qiong", CharTone.Second)]
            },

            // 茀: fu
            ['茀'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 苕: shao
            ['苕'] = new()
            {
                PinYins = [new("shao", CharTone.Second)]
            },

            // 枥: li
            ['枥'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 枇: pi
            ['枇'] = new()
            {
                PinYins = [new("pi", CharTone.Second)]
            },

            // 杪: miao
            ['杪'] = new()
            {
                PinYins = [new("miao", CharTone.Third)]
            },

            // 杳: yao
            ['杳'] = new()
            {
                PinYins = [new("yao", CharTone.Third)]
            },

            // 枧: jian
            ['枧'] = new()
            {
                PinYins = [new("jian", CharTone.Third)]
            },

            // 杵: chu
            ['杵'] = new()
            {
                PinYins = [new("chu", CharTone.Third)]
            },

            // 枨: cheng
            ['枨'] = new()
            {
                PinYins = [new("cheng", CharTone.Second)]
            },

            // 枞: cong
            ['枞'] = new()
            {
                PinYins = [new("cong", CharTone.First)]
            },

            // 枋: fang
            ['枋'] = new()
            {
                PinYins = [new("fang", CharTone.First)]
            },

            // 杻: chou
            ['杻'] = new()
            {
                PinYins = [new("chou", CharTone.Second)]
            },

            // 杷: pa
            ['杷'] = new()
            {
                PinYins = [new("pa", CharTone.Second)]
            },

            // 杼: zhu
            ['杼'] = new()
            {
                PinYins = [new("zhu", CharTone.Fourth)]
            },

            // 矸: gan
            ['矸'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 砀: dang
            ['砀'] = new()
            {
                PinYins = [new("dang", CharTone.Fourth)]
            },

            // 刳: ku
            ['刳'] = new()
            {
                PinYins = [new("ku", CharTone.First)]
            },

            // 奄: yan
            ['奄'] = new()
            {
                PinYins = [new("yan", CharTone.Third)]
            },

            // 瓯: ou
            ['瓯'] = new()
            {
                PinYins = [new("ou", CharTone.First)]
            },

            // 殁: mo
            ['殁'] = new()
            {
                PinYins = [new("mo", CharTone.Fourth)]
            },

            // 郏: jia
            ['郏'] = new()
            {
                PinYins = [new("jia", CharTone.Second)]
            },

            // 轭: e
            ['轭'] = new()
            {
                PinYins = [new("e", CharTone.Fourth)]
            },

            // 郅: zhi
            ['郅'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 鸢: yuan
            ['鸢'] = new()
            {
                PinYins = [new("yuan", CharTone.First)]
            },

            // 盱: xu
            ['盱'] = new()
            {
                PinYins = [new("xu", CharTone.First)]
            },

            // 昊: hao
            ['昊'] = new()
            {
                PinYins = [new("hao", CharTone.Fourth)]
            },

            // 昙: tan
            ['昙'] = new()
            {
                PinYins = [new("tan", CharTone.Second)]
            },

            // 杲: gao
            ['杲'] = new()
            {
                PinYins = [new("gao", CharTone.Third)]
            },

            // 昃: ze
            ['昃'] = new()
            {
                PinYins = [new("ze", CharTone.Fourth)]
            },

            // 咂: za
            ['咂'] = new()
            {
                PinYins = [new("za", CharTone.First)]
            },

            // 呸: pei
            ['呸'] = new()
            {
                PinYins = [new("pei", CharTone.First)]
            },

            // 昕: xin
            ['昕'] = new()
            {
                PinYins = [new("xin", CharTone.First)]
            },

            // 昀: yun
            ['昀'] = new()
            {
                PinYins = [new("yun", CharTone.Second)]
            },

            // 旻: min
            ['旻'] = new()
            {
                PinYins = [new("min", CharTone.Second)]
            },

            // 昉: fang
            ['昉'] = new()
            {
                PinYins = [new("fang", CharTone.Third)]
            },

            // 炅: jiong
            ['炅'] = new()
            {
                PinYins = [new("jiong", CharTone.Third)]
            },

            // 咔: ka
            ['咔'] = new()
            {
                PinYins = [new("ka", CharTone.First)]
            },

            // 畀: bi
            ['畀'] = new()
            {
                PinYins = [new("bi", CharTone.Fourth)]
            },

            // 虮: ji
            ['虮'] = new()
            {
                PinYins = [new("ji", CharTone.Third)]
            },

            // 咀: ju
            ['咀'] = new()
            {
                PinYins = [new("ju", CharTone.Third)]
            },

            // 呷: xia
            ['呷'] = new()
            {
                PinYins = [new("xia", CharTone.First)]
            },

            // 黾: min
            ['黾'] = new()
            {
                PinYins = [new("min", CharTone.Third)]
            },

            // 呱: gu
            ['呱'] = new()
            {
                PinYins = [new("gu", CharTone.First)]
            },

            // 呤: ling
            ['呤'] = new()
            {
                PinYins = [new("ling", CharTone.Second)]
            },

            // 咚: dong
            ['咚'] = new()
            {
                PinYins = [new("dong", CharTone.First)]
            },

            // 咆: pao
            ['咆'] = new()
            {
                PinYins = [new("pao", CharTone.Second)]
            },

            // 咛: ning
            ['咛'] = new()
            {
                PinYins = [new("ning", CharTone.Second)]
            },

            // 呶: nao
            ['呶'] = new()
            {
                PinYins = [new("nao", CharTone.Second)]
            },

            // 呣: m
            ['呣'] = new()
            {
                PinYins = [new("m", CharTone.Second)]
            },

            // 呦: you
            ['呦'] = new()
            {
                PinYins = [new("you", CharTone.First)]
            },

            // 咝: si
            ['咝'] = new()
            {
                PinYins = [new("si", CharTone.First)]
            },

            // 岢: ke
            ['岢'] = new()
            {
                PinYins = [new("ke", CharTone.Third)]
            },

            // 岿: kui
            ['岿'] = new()
            {
                PinYins = [new("kui", CharTone.First)]
            },

            // 岬: jia
            ['岬'] = new()
            {
                PinYins = [new("jia", CharTone.Third)]
            },

            // 岫: xiu
            ['岫'] = new()
            {
                PinYins = [new("xiu", CharTone.Fourth)]
            },

            // 帙: zhi
            ['帙'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 岣: gou
            ['岣'] = new()
            {
                PinYins = [new("gou", CharTone.Third)]
            },

            // 峁: mao
            ['峁'] = new()
            {
                PinYins = [new("mao", CharTone.Third)]
            },

            // 刿: gui
            ['刿'] = new()
            {
                PinYins = [new("gui", CharTone.Fourth)]
            },

            // 迥: jiong
            ['迥'] = new()
            {
                PinYins = [new("jiong", CharTone.Third)]
            },

            // 岷: min
            ['岷'] = new()
            {
                PinYins = [new("min", CharTone.Second)]
            },

            // 剀: kai
            ['剀'] = new()
            {
                PinYins = [new("kai", CharTone.Third)]
            },

            // 帔: pei
            ['帔'] = new()
            {
                PinYins = [new("pei", CharTone.Fourth)]
            },

            // 峄: yi
            ['峄'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 沓: ta
            ['沓'] = new()
            {
                PinYins = [new("ta", CharTone.Fourth)]
            },

            // 囹: ling
            ['囹'] = new()
            {
                PinYins = [new("ling", CharTone.Second)]
            },

            // 罔: wang
            ['罔'] = new()
            {
                PinYins = [new("wang", CharTone.Third)]
            },

            // 钍: tu
            ['钍'] = new()
            {
                PinYins = [new("tu", CharTone.Third)]
            },

            // 钎: qian
            ['钎'] = new()
            {
                PinYins = [new("qian", CharTone.First)]
            },

            // 钏: chuan
            ['钏'] = new()
            {
                PinYins = [new("chuan", CharTone.Fourth)]
            },

            // 钒: fan
            ['钒'] = new()
            {
                PinYins = [new("fan", CharTone.Second)]
            },

            // 钕: nv
            ['钕'] = new()
            {
                PinYins = [new("nv", CharTone.Third)]
            },

            // 钗: chai
            ['钗'] = new()
            {
                PinYins = [new("chai", CharTone.First)]
            },

            // 邾: zhu
            ['邾'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 迮: ze
            ['迮'] = new()
            {
                PinYins = [new("ze", CharTone.Second)]
            },

            // 牦: mao
            ['牦'] = new()
            {
                PinYins = [new("mao", CharTone.Second)]
            },

            // 竺: zhu
            ['竺'] = new()
            {
                PinYins = [new("zhu", CharTone.Second)]
            },

            // 迤: yi
            ['迤'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 佶: ji
            ['佶'] = new()
            {
                PinYins = [new("ji", CharTone.Second)]
            },

            // 佬: lao
            ['佬'] = new()
            {
                PinYins = [new("lao", CharTone.Third)]
            },

            // 佰: bai
            ['佰'] = new()
            {
                PinYins = [new("bai", CharTone.Third)]
            },

            // 侑: you
            ['侑'] = new()
            {
                PinYins = [new("you", CharTone.Fourth)]
            },

            // 侉: kua
            ['侉'] = new()
            {
                PinYins = [new("kua", CharTone.Third)]
            },

            // 臾: yu
            ['臾'] = new()
            {
                PinYins = [new("yu", CharTone.Second)]
            },

            // 岱: dai
            ['岱'] = new()
            {
                PinYins = [new("dai", CharTone.Fourth)]
            },

            // 侗: tong
            ['侗'] = new()
            {
                PinYins = [new("tong", CharTone.Second)]
            },

            // 侃: kan
            ['侃'] = new()
            {
                PinYins = [new("kan", CharTone.Third)]
            },

            // 侏: zhu
            ['侏'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 侩: kuai
            ['侩'] = new()
            {
                PinYins = [new("kuai", CharTone.Fourth)]
            },

            // 佻: tiao
            ['佻'] = new()
            {
                PinYins = [new("tiao", CharTone.First)]
            },

            // 佾: yi
            ['佾'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 侪: chai
            ['侪'] = new()
            {
                PinYins = [new("chai", CharTone.Second)]
            },

            // 佼: jiao
            ['佼'] = new()
            {
                PinYins = [new("jiao", CharTone.Third)]
            },

            // 佯: yang
            ['佯'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 侬: nong
            ['侬'] = new()
            {
                PinYins = [new("nong", CharTone.Second)]
            },

            // 帛: bo
            ['帛'] = new()
            {
                PinYins = [new("bo", CharTone.Second)]
            },

            // 阜: fu
            ['阜'] = new()
            {
                PinYins = [new("fu", CharTone.Fourth)]
            },

            // 侔: mou
            ['侔'] = new()
            {
                PinYins = [new("mou", CharTone.Second)]
            },

            // 徂: cu
            ['徂'] = new()
            {
                PinYins = [new("cu", CharTone.Second)]
            },

            // 刽: gui
            ['刽'] = new()
            {
                PinYins = [new("gui", CharTone.Fourth)]
            },

            // 郄: qie
            ['郄'] = new()
            {
                PinYins = [new("qie", CharTone.Fourth)]
            },

            // 怂: song
            ['怂'] = new()
            {
                PinYins = [new("song", CharTone.Third)]
            },

            // 籴: di
            ['籴'] = new()
            {
                PinYins = [new("di", CharTone.Second)]
            },

            // 瓮: weng
            ['瓮'] = new()
            {
                PinYins = [new("weng", CharTone.Fourth)]
            },

            // 戗: qiang
            ['戗'] = new()
            {
                PinYins = [new("qiang", CharTone.Fourth)]
            },

            // 肼: jing
            ['肼'] = new()
            {
                PinYins = [new("jing", CharTone.Third)]
            },

            // 䏝: zhi
            ['䏝'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 肽: tai
            ['肽'] = new()
            {
                PinYins = [new("tai", CharTone.Fourth)]
            },

            // 肱: gong
            ['肱'] = new()
            {
                PinYins = [new("gong", CharTone.First)]
            },

            // 肫: zhun
            ['肫'] = new()
            {
                PinYins = [new("zhun", CharTone.First)]
            },

            // 剁: duo
            ['剁'] = new()
            {
                PinYins = [new("duo", CharTone.Fourth)]
            },

            // 迩: er
            ['迩'] = new()
            {
                PinYins = [new("er", CharTone.Third)]
            },

            // 郇: xun
            ['郇'] = new()
            {
                PinYins = [new("xun", CharTone.Second)]
            },

            // 狙: ju
            ['狙'] = new()
            {
                PinYins = [new("ju", CharTone.First)]
            },

            // 狎: xia
            ['狎'] = new()
            {
                PinYins = [new("xia", CharTone.Second)]
            },

            // 狍: pao
            ['狍'] = new()
            {
                PinYins = [new("pao", CharTone.Second)]
            },

            // 狒: fei
            ['狒'] = new()
            {
                PinYins = [new("fei", CharTone.Fourth)]
            },

            // 咎: jiu
            ['咎'] = new()
            {
                PinYins = [new("jiu", CharTone.Fourth)]
            },

            // 炙: zhi
            ['炙'] = new()
            {
                PinYins = [new("zhi", CharTone.Fourth)]
            },

            // 枭: xiao
            ['枭'] = new()
            {
                PinYins = [new("xiao", CharTone.First)]
            },

            // 饯: jian
            ['饯'] = new()
            {
                PinYins = [new("jian", CharTone.Fourth)]
            },

            // 饴: yi
            ['饴'] = new()
            {
                PinYins = [new("yi", CharTone.Second)]
            },

            // 冽: lie
            ['冽'] = new()
            {
                PinYins = [new("lie", CharTone.Fourth)]
            },

            // 冼: xian
            ['冼'] = new()
            {
                PinYins = [new("xian", CharTone.Third)]
            },

            // 庖: pao
            ['庖'] = new()
            {
                PinYins = [new("pao", CharTone.Second)]
            },

            // 疠: li
            ['疠'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 疝: shan
            ['疝'] = new()
            {
                PinYins = [new("shan", CharTone.Fourth)]
            },

            // 疡: yang
            ['疡'] = new()
            {
                PinYins = [new("yang", CharTone.Second)]
            },

            // 兖: yan
            ['兖'] = new()
            {
                PinYins = [new("yan", CharTone.Third)]
            },

            // 妾: qie
            ['妾'] = new()
            {
                PinYins = [new("qie", CharTone.Fourth)]
            },

            // 劾: he
            ['劾'] = new()
            {
                PinYins = [new("he", CharTone.Second)]
            },

            // 炜: wei
            ['炜'] = new()
            {
                PinYins = [new("wei", CharTone.Third)]
            },

            // 炖: dun
            ['炖'] = new()
            {
                PinYins = [new("dun", CharTone.Fourth)]
            },

            // 炘: xin
            ['炘'] = new()
            {
                PinYins = [new("xin", CharTone.First)]
            },

            // 炝: qiang
            ['炝'] = new()
            {
                PinYins = [new("qiang", CharTone.Fourth)]
            },

            // 炔: que
            ['炔'] = new()
            {
                PinYins = [new("que", CharTone.Fourth)]
            },

            // 泔: gan
            ['泔'] = new()
            {
                PinYins = [new("gan", CharTone.First)]
            },

            // 沭: shu
            ['沭'] = new()
            {
                PinYins = [new("shu", CharTone.Fourth)]
            },

            // 泷: long
            ['泷'] = new()
            {
                PinYins = [new("long", CharTone.Second)]
            },

            // 泸: lu
            ['泸'] = new()
            {
                PinYins = [new("lu", CharTone.Second)]
            },

            // 泱: yang
            ['泱'] = new()
            {
                PinYins = [new("yang", CharTone.First)]
            },

            // 泅: qiu
            ['泅'] = new()
            {
                PinYins = [new("qiu", CharTone.Second)]
            },

            // 泗: si
            ['泗'] = new()
            {
                PinYins = [new("si", CharTone.Fourth)]
            },

            // 泠: ling
            ['泠'] = new()
            {
                PinYins = [new("ling", CharTone.Second)]
            },

            // 泺: luo
            ['泺'] = new()
            {
                PinYins = [new("luo", CharTone.Fourth)]
            },

            // 泖: mao
            ['泖'] = new()
            {
                PinYins = [new("mao", CharTone.Third)]
            },

            // 泫: xuan
            ['泫'] = new()
            {
                PinYins = [new("xuan", CharTone.Third)]
            },

            // 泮: pan
            ['泮'] = new()
            {
                PinYins = [new("pan", CharTone.Fourth)]
            },

            // 沱: tuo
            ['沱'] = new()
            {
                PinYins = [new("tuo", CharTone.Second)]
            },

            // 泯: min
            ['泯'] = new()
            {
                PinYins = [new("min", CharTone.Third)]
            },

            // 泓: hong
            ['泓'] = new()
            {
                PinYins = [new("hong", CharTone.Second)]
            },

            // 泾: jing
            ['泾'] = new()
            {
                PinYins = [new("jing", CharTone.First)]
            },

            // 怙: hu
            ['怙'] = new()
            {
                PinYins = [new("hu", CharTone.Fourth)]
            },

            // 怵: chu
            ['怵'] = new()
            {
                PinYins = [new("chu", CharTone.Fourth)]
            },

            // 怦: peng
            ['怦'] = new()
            {
                PinYins = [new("peng", CharTone.First)]
            },

            // 怛: da
            ['怛'] = new()
            {
                PinYins = [new("da", CharTone.Second)]
            },

            // 怏: yang
            ['怏'] = new()
            {
                PinYins = [new("yang", CharTone.Fourth)]
            },

            // 怍: zuo
            ['怍'] = new()
            {
                PinYins = [new("zuo", CharTone.Fourth)]
            },

            // 㤘: ni
            ['㤘'] = new()
            {
                PinYins = [new("ni", CharTone.Second)]
            },

            // 怩: ni
            ['怩'] = new()
            {
                PinYins = [new("ni", CharTone.Second)]
            },

            // 怫: fu
            ['怫'] = new()
            {
                PinYins = [new("fu", CharTone.Second)]
            },

            // 怿: yi
            ['怿'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 宕: dang
            ['宕'] = new()
            {
                PinYins = [new("dang", CharTone.Fourth)]
            },

            // 穹: qiong
            ['穹'] = new()
            {
                PinYins = [new("qiong", CharTone.Second)]
            },

            // 宓: mi
            ['宓'] = new()
            {
                PinYins = [new("mi", CharTone.Fourth)]
            },

            // 诓: kuang
            ['诓'] = new()
            {
                PinYins = [new("kuang", CharTone.First)]
            },

            // 诔: lei
            ['诔'] = new()
            {
                PinYins = [new("lei", CharTone.Third)]
            },

            // 诖: gua
            ['诖'] = new()
            {
                PinYins = [new("gua", CharTone.Fourth)]
            },

            // 诘: jie
            ['诘'] = new()
            {
                PinYins = [new("jie", CharTone.Second)]
            },

            // 戾: li
            ['戾'] = new()
            {
                PinYins = [new("li", CharTone.Fourth)]
            },

            // 诙: hui
            ['诙'] = new()
            {
                PinYins = [new("hui", CharTone.First)]
            },

            // 戽: hu
            ['戽'] = new()
            {
                PinYins = [new("hu", CharTone.Fourth)]
            },

            // 郓: yun
            ['郓'] = new()
            {
                PinYins = [new("yun", CharTone.Fourth)]
            },

            // 衩: cha
            ['衩'] = new()
            {
                PinYins = [new("cha", CharTone.Third)]
            },

            // 祆: xian
            ['祆'] = new()
            {
                PinYins = [new("xian", CharTone.First)]
            },

            // 祎: yi
            ['祎'] = new()
            {
                PinYins = [new("yi", CharTone.First)]
            },

            // 祉: zhi
            ['祉'] = new()
            {
                PinYins = [new("zhi", CharTone.Third)]
            },

            // 祇: qi
            ['祇'] = new()
            {
                PinYins = [new("qi", CharTone.Second)]
            },

            // 诛: zhu
            ['诛'] = new()
            {
                PinYins = [new("zhu", CharTone.First)]
            },

            // 诜: shen
            ['诜'] = new()
            {
                PinYins = [new("shen", CharTone.First)]
            },

            // 诟: gou
            ['诟'] = new()
            {
                PinYins = [new("gou", CharTone.Fourth)]
            },

            // 诠: quan
            ['诠'] = new()
            {
                PinYins = [new("quan", CharTone.Second)]
            },

            // 诣: yi
            ['诣'] = new()
            {
                PinYins = [new("yi", CharTone.Fourth)]
            },

            // 诤: zheng
            ['诤'] = new()
            {
                PinYins = [new("zheng", CharTone.Fourth)]
            },

            // 诧: cha
            ['诧'] = new()
            {
                PinYins = [new("cha", CharTone.Fourth)]
            },

            // 诨: hun
            ['诨'] = new()
            {
                PinYins = [new("hun", CharTone.Fourth)]
            }
        };
    }
}
