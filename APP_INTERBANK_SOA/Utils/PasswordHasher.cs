using System.Security.Cryptography;
using System.Text;


namespace APP_INTERBANK_SOA.Utils
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;   
        private const int KeySize = 32;    
        private const int Iterations = 10;
        private const string Algorithm = "PBKDF2-SHA256";

        // Genera un hash con salt incluida
        public static string Hash(string password)
        {
            // 1. Generar salt aleatoria
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            // 2. Derivar clave con PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256
            );

            var key = pbkdf2.GetBytes(KeySize);

            // 3. Guardamos todo en un solo string
            return string.Join(
                '.',
                Algorithm,
                Iterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key)
            );
        }

        // Verifica si la password coincide con el hash guardado
        public static bool Verify(string password, string hash)
        {
            var parts = hash.Split('.', 4);
            if (parts.Length != 4) return false;

            var algorithm = parts[0];
            if (algorithm != Algorithm) return false; // por si cambias versión en el futuro

            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var key = Convert.FromBase64String(parts[3]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256
            );

            var keyToCheck = pbkdf2.GetBytes(key.Length);

            // Comparación en tiempo constante
            return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
        }
    }
}
