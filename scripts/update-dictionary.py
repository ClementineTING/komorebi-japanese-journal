import json,pathlib
import urllib.request,tarfile,io
root=pathlib.Path(__file__).resolve().parents[1]
request=urllib.request.Request('https://api.github.com/repos/scriptin/jmdict-simplified/releases/latest',headers={'User-Agent':'Komorebi-dictionary-updater'})
release=json.load(urllib.request.urlopen(request))
asset=next(a for a in release['assets'] if 'eng-common' in a['name'] and a['name'].endswith('.tgz'))
with tarfile.open(fileobj=io.BytesIO(urllib.request.urlopen(asset['browser_download_url']).read())) as archive:
    member=next(m for m in archive.getmembers() if m.name.endswith('.json'))
    raw=json.load(archive.extractfile(member))
words=[]
for w in raw['words']:
 k=w['kanji']; r=w['kana']; senses=w['sense']
 words.append({'id':w['id'],'w':next((x['text'] for x in k if x['common']),k[0]['text'] if k else r[0]['text']),'r':r[0]['text'],'en':' / '.join('; '.join(g['text'] for g in s['gloss']) for s in senses),'pos':', '.join(dict.fromkeys(p for s in senses for p in s['partOfSpeech'])),'aliases':' '.join(x['text'] for x in k+r)})
rows='''日差し|ひざし|阳光；日照|春の日差しは暖かいです。|春天的阳光很温暖。
食べる|たべる|吃|毎朝、パンを食べます。|我每天早上吃面包。
飲む|のむ|喝；饮用|温かいお茶を飲みます。|喝一杯热茶。
学校|がっこう|学校|歩いて学校に行きます。|我走路去学校。
友達|ともだち|朋友|週末、友達に会います。|周末和朋友见面。
天気|てんき|天气|今日はいい天気ですね。|今天天气真好啊。
猫|ねこ|猫|猫が窓のそばで寝ています。|猫正在窗边睡觉。
読む|よむ|读；阅读|寝る前に本を読みます。|睡觉前读书。
静か|しずか|安静；宁静|ここは静かな町です。|这里是一个安静的小镇。
好き|すき|喜欢|日本の音楽が好きです。|我喜欢日本音乐。
時間|じかん|时间|少し時間があります。|我有一点时间。
歩く|あるく|走；步行|毎日、公園を歩きます。|我每天在公园散步。
雨|あめ|雨|午後から雨が降ります。|下午开始下雨。
空|そら|天空|青い空を見上げます。|抬头看蓝天。
水|みず|水|水を一杯ください。|请给我一杯水。
電車|でんしゃ|电车|電車で会社に行きます。|我坐电车去公司。
新しい|あたらしい|新的|新しい靴を買いました。|我买了新鞋。
楽しい|たのしい|快乐的；愉快的|旅行はとても楽しかったです。|旅行非常愉快。
勉強|べんきょう|学习|毎日、日本語を勉強します。|我每天学习日语。
明日|あした|明天|明日、また会いましょう。|明天再见吧。'''
ids=[]
for row in rows.splitlines():
 w,r,zh,ja,cn=row.split('|'); match=next((x for x in words if w==x['w'] or w in x['aliases'].split()),None)
 if match:
  match.update(zh=zh,ex=ja,cn=cn,r=r);ids.append(match['id'])
 else: print('missing',w)
html=(root/'web/template.html').read_text();html=html.replace('__DATA__',json.dumps(words,ensure_ascii=False,separators=(',',':')).replace('</','<\\/')).replace('__DECK__',json.dumps(ids))
(root/'web/index.html').write_text(html.replace('22,639',format(len(words),',')).replace('2026-09-07',raw['dictDate']))
print(len(words),len(ids),len(html.encode()))
