namespace NetCoreERPSys.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }

        public double OrderTotal { get; set; }
    }
}
