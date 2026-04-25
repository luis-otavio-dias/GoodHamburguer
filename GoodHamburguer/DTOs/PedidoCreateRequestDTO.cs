namespace GoodHamburguer.DTOs;

public record PedidoCreateRequestDTO(
    int SanduicheId,
    List<int> AcompanhamentoIds
);