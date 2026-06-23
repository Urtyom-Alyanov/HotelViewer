namespace HotelViewer.Presentation.Windows.Editors;

public interface IEntityEditor<TEntity> {
  public TEntity? Entity { get; }

  /// <summary>
  /// Метод предзаполнения формы данными для редактирования
  /// </summary>
  /// <param name="entity">Сущность</param>
  public void SetEntity(TEntity entity);

  public bool? ShowDialog();
}
