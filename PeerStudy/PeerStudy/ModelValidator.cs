namespace PeerStudy
{
    public static class ModelValidator
    {
        public static bool IsModelValid<T>(T model) where T : class
        {
            if (model == null)
            {
                return false;
            }

            bool isValid = true;

            var type = model.GetType();
            foreach (var property in type.GetProperties())
            {
                if (property.GetValue(model) == null)
                {
                    return false;
                }
            }

            return isValid;
        }
    }
}
