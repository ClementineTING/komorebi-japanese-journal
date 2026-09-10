const DynamicDictionaries=require('kuromoji/src/dict/DynamicDictionaries');
const Tokenizer=require('kuromoji/src/Tokenizer');
const {gunzipSync}=require('fflate');
let tokenizer;
self.onmessage=e=>{try{if(e.data.type==='init'){
const packed=e.data.dicts;
const u8=n=>gunzipSync(Uint8Array.from(atob(packed[n+'.dat.gz']),c=>c.charCodeAt(0)));
const dic=new DynamicDictionaries();dic.loadTrie(new Int32Array(u8('base').buffer),new Int32Array(u8('check').buffer));
dic.loadTokenInfoDictionaries(u8('tid'),u8('tid_pos'),u8('tid_map'));dic.loadConnectionCosts(new Int16Array(u8('cc').buffer));
dic.loadUnknownDictionaries(u8('unk'),u8('unk_pos'),u8('unk_map'),u8('unk_char'),new Uint32Array(u8('unk_compat').buffer),u8('unk_invoke'));
tokenizer=new Tokenizer(dic);self.postMessage({type:'ready'});
}else if(e.data.type==='annotate'){self.postMessage({type:'result',id:e.data.id,tokens:tokenizer.tokenize(e.data.text).map(t=>({text:t.surface_form,reading:t.reading||'',basic:t.basic_form}))})}}
catch(err){self.postMessage({type:'error',id:e.data.id,error:err.message})}};
