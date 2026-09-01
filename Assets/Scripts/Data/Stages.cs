using System;
using System.Collections.Generic;

namespace OwnADungeon.Data
{
    // Direct port of src/data/stages.ts — handcrafted stage 1-50 progression.
    public static class Stages
    {
        static readonly List<string> Tutorial = new List<string> { "mage", "berserker" };
        static readonly List<string> OpenWarrior = new List<string> { "mage", "berserker", "warrior" };
        static readonly List<string> OpenPaladin = new List<string> { "mage", "berserker", "warrior", "paladin" };
        static readonly List<string> FullRoster = new List<string> { "mage", "berserker", "warrior", "paladin", "rogue" };

        public static readonly List<StageDef> Defs = new List<StageDef>
        {
            new StageDef { Stage = 1, HeroPool = new List<string> { "mage" }, Note = "Tutorial: hanya Mage yang menyerang. Spike Trap saja sudah cukup — Mage sangat rentan padanya." },
            new StageDef { Stage = 2, HeroPool = new List<string> { "berserker" }, Note = "Tutorial: hanya Berserker yang menyerang. Skeleton Archer meredam RAGE-nya lewat chip damage bertahap." },
            new StageDef { Stage = 3, HeroPool = Tutorial, Note = "Mage dan Berserker bisa muncul bergantian. Taklukkan ini untuk membuka Poison Trap + Goblin Brute." },
            new StageDef { Stage = 4, HeroPool = Tutorial, Note = "Toolkit sama, tapi kombinasikan Spike dan Skeleton di ruang yang berbeda untuk melihat urutan encounter." },
            new StageDef { Stage = 5, HeroPool = Tutorial, Note = "Ujian akhir tutorial. Taklukkan ini untuk membuka Ruang ke-4." },
            new StageDef { Stage = 6, HeroPool = OpenWarrior, Note = "Warrior mulai muncul — ia tahan Spike, jadi andalkan Poison Trap yang baru terbuka untuk menembus DEF-nya." },
            new StageDef { Stage = 7, HeroPool = OpenWarrior, Note = "Goblin Brute (baru terbuka) adalah matchup terburuk Mage — pasang di ruang awal untuk raid Mage." },
            new StageDef { Stage = 8, HeroPool = OpenPaladin, Note = "Paladin muncul — nyaris serentan Warrior terhadap Poison. Taklukkan ini untuk membuka Net Trap." },
            new StageDef { Stage = 9, HeroPool = FullRoster, Note = "Rogue melengkapi roster — ia biasa lolos dari trap fisik, tapi Net Trap yang baru terbuka dibuat khusus untuk menjeratnya." },
            new StageDef { Stage = 10, HeroPool = FullRoster, Note = "Roster penuh 5 kelas. Urutan ruang menentukan: taruh counter yang tepat di ruang pertama yang ditemui hero." },
            new StageDef { Stage = 11, HeroPool = FullRoster, Note = "Kombinasi Goblin Brute + Poison Trap dalam satu layout menutup celah Warrior sekaligus Mage." },
            new StageDef { Stage = 12, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Fire Trap — damage bakar susulan yang menghukum hero yang bertahan lama di ruang awal." },
            new StageDef { Stage = 13, HeroPool = FullRoster, Note = "Fire Trap yang baru terbuka: efek bakarnya terus jalan setelah ruang berikutnya dimulai — manfaatkan itu." },
            new StageDef { Stage = 14, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Acid Slime — dinding fisik-resistant yang meredam hero non-magic." },
            new StageDef { Stage = 15, HeroPool = FullRoster, Note = "Acid Slime (baru terbuka) paling efektif di belakang trap yang sudah melemahkan hero lebih dulu." },
            new StageDef { Stage = 16, HeroPool = FullRoster, Note = "Timing: Poison Trap terus mencicil HP lewat DOT — biarkan racun bekerja sebelum ruang monster berikutnya." },
            new StageDef { Stage = 17, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Frost Trap — mengurangi DEF hero sehingga ruang monster sesudahnya menghantam lebih keras." },
            new StageDef { Stage = 18, HeroPool = FullRoster, Note = "Kombinasi baru: Frost Trap (DEF turun) diikuti Goblin Brute (burst ATK) adalah combo dua langkah." },
            new StageDef { Stage = 19, HeroPool = FullRoster, Note = "Review komposisi: pastikan trap dan monster yang kamu pasang benar-benar menutup kelemahan hero yang mungkin datang." },
            new StageDef { Stage = 20, HeroPool = FullRoster, Note = "Semua trap dan monster yang sudah terbuka sejauh ini harus saling melengkapi dalam satu layout." },
            new StageDef { Stage = 21, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Bone Ogre — tank yang paling menyulitkan Rogue (matchup terburuknya)." },
            new StageDef { Stage = 22, HeroPool = FullRoster, Note = "Bone Ogre (baru terbuka) cocok di ruang belakang; biarkan trap cepat menyaring di ruang depan." },
            new StageDef { Stage = 23, HeroPool = FullRoster, Note = "Urutan: trap ringan dulu untuk memancing reaksi (panic/flee), monster berat di ruang terakhir sebelum Throne." },
            new StageDef { Stage = 24, HeroPool = FullRoster, Note = "Campurkan sumber damage fisik dan DOT — jangan andalkan satu jenis saja karena tiap kelas hero punya resistansi berbeda." },
            new StageDef { Stage = 25, HeroPool = FullRoster, Note = "Raid lebih panjang di stage ini menguntungkan Frost Trap: DEF yang berkurang bertahan sepanjang sisa raid." },
            new StageDef { Stage = 26, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Shadow Wraith — aura takutnya menghukum semua kelas kecuali Berserker dan Paladin (fear-immune)." },
            new StageDef { Stage = 27, HeroPool = FullRoster, Note = "Shadow Wraith (baru terbuka) paling efektif melawan Warrior/Rogue/Mage — sia-sia dipasang untuk menghadapi Berserker atau Paladin." },
            new StageDef { Stage = 28, HeroPool = FullRoster, Note = "Lapisan combo: Poison Trap terus mencicil HP sementara efek takut dari Shadow Wraith menurunkan ATK hero." },
            new StageDef { Stage = 29, HeroPool = FullRoster, Note = "Posisi: taruh Shadow Wraith sebelum ruang tersulit supaya efek takutnya melemahkan hero lebih dulu." },
            new StageDef { Stage = 30, HeroPool = FullRoster, Note = "Review penuh sebelum unlock terakhir: seluruh trap dan monster yang sudah terbuka harus sinergi dalam satu dungeon." },
            new StageDef { Stage = 31, HeroPool = FullRoster, Note = "Satu stage lagi sebelum Ruang ke-5 terbuka — manfaatkan 4 ruang yang ada semaksimal mungkin." },
            new StageDef { Stage = 32, HeroPool = FullRoster, Note = "Taklukkan ini untuk membuka Ruang ke-5, perluasan dungeon terakhir." },
            new StageDef { Stage = 33, HeroPool = FullRoster, Note = "Layout 5-ruang dimulai di sini — rencanakan alur penuh, bukan cuma ruang pembuka yang kuat." },
            new StageDef { Stage = 34, HeroPool = FullRoster, Note = "Variasi counter: hindari memasang jenis trap/monster yang sama dua ruang berturut-turut." },
            new StageDef { Stage = 35, HeroPool = new List<string> { "berserker" }, Note = "Gauntlet Berserker: fear-immune dan sulit panik — andalkan DOT (Poison/Fire) yang tetap mencicil terlepas dari RAGE-nya." },
            new StageDef { Stage = 36, HeroPool = new List<string> { "mage" }, Note = "Gauntlet Mage: rapuh tapi berbahaya — tempatkan Spike/Poison/Goblin Brute di ruang-ruang awal sebelum ia mencapai Throne." },
            new StageDef { Stage = 37, HeroPool = new List<string> { "rogue" }, Note = "Gauntlet Rogue: evasion tinggi terhadap trap fisik — kombinasi Net Trap (menjerat) dan Bone Ogre (matchup terburuknya) menutup celahnya." },
            new StageDef { Stage = 38, HeroPool = new List<string> { "warrior" }, Note = "Gauntlet Warrior: HP dan DEF tebal di garis depan — Poison Trap yang terus mencicil lebih efektif daripada damage instan." },
            new StageDef { Stage = 39, HeroPool = new List<string> { "paladin" }, Note = "Gauntlet Paladin: fear-immune dan holy vs undead/ethereal — Shadow Wraith sia-sia di sini, andalkan Poison dan Goblin Brute." },
            new StageDef { Stage = 40, HeroPool = FullRoster, Note = "Roster campuran kembali terbuka — satu layout harus menjawab beberapa kemungkinan kelas hero sekaligus." },
            new StageDef { Stage = 41, HeroPool = FullRoster, Note = "Combo review: Frost Trap ke Fire Trap — DEF berkurang dulu, lalu damage bakar menembus lebih dalam." },
            new StageDef { Stage = 42, HeroPool = FullRoster, Note = "Combo review: Net Trap ke Bone Ogre — kelas evasif dijerat dulu, baru dihadang tank yang sulit ia lewati." },
            new StageDef { Stage = 43, HeroPool = FullRoster, Note = "Combo review: Poison Trap ke Shadow Wraith — DOT terus berjalan sementara efek takut menekan ATK sepanjang raid." },
            new StageDef { Stage = 44, HeroPool = FullRoster, Note = "Seluruh 5 trap dan 5 monster yang sudah terbuka kini bisa dirotasi bebas — rancang layout paling efisien." },
            new StageDef { Stage = 45, HeroPool = FullRoster, Note = "Roster penuh, tanpa petunjuk kelas mana yang datang — susun dungeon yang menutup kelemahan kelima kelas sekaligus." },
            new StageDef { Stage = 46, HeroPool = FullRoster, Note = "Tekanan menuju Throne meningkat — pastikan HP hero sudah terkuras signifikan sebelum ia bertemu King." },
            new StageDef { Stage = 47, HeroPool = FullRoster, Note = "Presisi: hanya layout dengan urutan counter yang tepat yang menyelesaikan stage ini dengan bersih." },
            new StageDef { Stage = 48, HeroPool = FullRoster, Note = "Mastery check: rotasikan seluruh 5 trap dan 5 monster dalam satu raid 5-ruang." },
            new StageDef { Stage = 49, HeroPool = FullRoster, Note = "Gauntlet terakhir sebelum puncak — setiap mekanik yang pernah diperkenalkan bisa muncul dalam satu raid." },
            new StageDef { Stage = 50, HeroPool = FullRoster, Note = "Mahakarya Dungeon: puzzle penuh yang menggabungkan setiap trap, monster, dan combo yang pernah dibuka." }
        };

        public static StageDef GetStageDef(int stage)
        {
            int s = Math.Max(1, Math.Min(Defs.Count, stage));
            return Defs[s - 1];
        }
    }
}
