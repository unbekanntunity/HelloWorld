import React, { Component, createRef } from 'react';

import { Button } from './Button';

import add from '../images/add-circle.png';
import remove from '../images/delete.png';

import './ImageSection.css';

class ImageSection extends Component {
    state = {
        preview: [],
        images: [],
        deleteMode: false,
        selectedToRemove: []
    }

    constructor(props) {
        super(props);
        this.hiddenInputRef = createRef();
    }

    getImages = () => {
        return this.state.images;
    }

    handleChange = (event) => {
        const addedImages = [...this.state.images, ...event.target.files];

        this.setState({
            images: addedImages,
            preview: addedImages.map(item => URL.createObjectURL(item)) 
        })

        this.hiddenInputRef.current.value = ""

        setTimeout(() => console.log(this.state.images)); 
    };

    handleOnToggleButtonClick = () => {
        this.hiddenInputRef.current.click();
    }

    handleToggleRemove = () => {
        if (this.state.deleteMode) {
            let newImageArrary = [];

            for (let i = 0; i < this.state.preview.length; i++) {
                if (!this.state.selectedToRemove.includes(this.state.preview[i])) {
                    newImageArrary = this.state.images[i];
                }
            }

            this.setState({
                preview: this.state.preview.filter(item => !this.state.selectedToRemove.includes(item)),
                images: newImageArrary,
                selectedToRemove: [],
            })
        }
   
        this.setState({
            deleteMode: !this.state.deleteMode
        })
    }

    handleItemSelectToRemove = (item) => {
        if (this.state.selectedToRemove.indexOf(item) === -1) {
            this.setState({ selectedToRemove: [...this.state.selectedToRemove, item] })
        }
        else {
            this.setState({ selectedToRemove: this.state.selectedToRemove.filter(itemInArray => itemInArray !== item) })
        }
    }

    handleSelectAllToRemove = () => {
        this.setState({ selectedToRemove: [...this.state.selectedToRemove, ...this.state.preview] })
    }

    render() {
        return (
            <div>
                <div className="section-header">
                    <p className="section-header-text">Images</p>
                    <img className="imageSection-toggle" src={add} alt="" width={20}
                        onClick={this.handleOnToggleButtonClick} />
                    {
                        this.state.deleteMode &&
                        <div className="imageSection-deleteAll">
                            <Button text="Select all" onClick={this.handleSelectAllToRemove} />
                        </div>
                    }
                </div>
                <div className="imageSection-images">
                    {
                        this.state.preview.map((item, index) =>
                            <div key={index} style={{ height: 40 }} onClick={() => this.handleItemSelectToRemove(item)}>
                                <img className="imageSection-image" src={item} alt="" width={this.props.imageSize} />
                                <div className="imageSection-image-delete" style={{
                                    display: this.state.deleteMode ? 'block' : 'none',
                                    background: this.state.selectedToRemove.indexOf(item) === -1 ? 'white' : '#FFB30B'
                                }}></div>
                            </div>
                        )
                    }
                    {
                        this.state.preview.length > 0 &&
                        <div className={this.state.deleteMode ? "imageSection-deleteOn" : "imageSection-deleteOff "}>
                            <img src={remove} alt="" width={20} height={20}
                                onClick={this.handleToggleRemove} />
                        </div>
                    }
                    
                </div>
                <input ref={this.hiddenInputRef} className="imageSection-input" type="file" accept="image/*" multiple={true}
                    onInput={this.handleChange} />
              
            </div>
        )
    }
}

export default ImageSection;