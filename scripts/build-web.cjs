const fs=require('fs'),path=require('path'),esbuild=require('esbuild');
process.chdir(path.join(__dirname,'..'));fs.mkdirSync('build',{recursive:true});
esbuild.buildSync({entryPoints:['web/furigana-worker.js'],bundle:true,platform:'browser',format:'iife',minify:true,outfile:'build/worker.bundle.js'});
const dicts={};for(const f of fs.readdirSync('node_modules/kuromoji/dict'))if(f.endsWith('.gz'))dicts[f]=fs.readFileSync('node_modules/kuromoji/dict/'+f).toString('base64');
const safe=s=>s.replace(/<\//g,'<\\/');let html=fs.readFileSync('web/template.html','utf8').replace('__DATA__',safe(fs.readFileSync('data/vocabulary.json','utf8'))).replace('__WORKER__',safe(fs.readFileSync('build/worker.bundle.js','utf8'))).replace('__DICTS__',JSON.stringify(dicts));fs.writeFileSync('web/index.html',html);console.log('Built offline HTML:',Buffer.byteLength(html),'bytes');
